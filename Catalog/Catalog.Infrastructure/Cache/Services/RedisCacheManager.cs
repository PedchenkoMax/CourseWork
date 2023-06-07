using System.Reflection;
using Catalog.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

namespace Catalog.Infrastructure.Cache.Services;

public class RedisCacheManager
{
    private readonly ILogger<RedisCacheManager> logger;
    private readonly JsonSerializerSettings jsonSettings;

    public RedisCacheManager(ILogger<RedisCacheManager> logger)
    {
        this.logger = logger;
        jsonSettings = new JsonSerializerSettings
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new PrivateResolver()
        };
    }

    public async Task<T> GetFromCacheAsync<T>(IDatabase database, string key, Func<Task<T>> fetchFromDb, TimeSpan? expiry = null)
    {
        try
        {
            var value = await database.StringGetAsync(key);

            if (!value.IsNull)
            {
                logger.LogInformation("Cache hit for key: {Key}", key);
                return JsonConvert.DeserializeObject<T>(value, jsonSettings);
            }

            logger.LogInformation("Cache miss for key: {Key}", key);

            var result = await fetchFromDb();

            await database.StringSetAsync(key, JsonConvert.SerializeObject(result, jsonSettings), expiry);
            logger.LogInformation("Item stored in cache for key: {Key}", key);

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while getting data from the cache: {Message}", ex.Message);
            throw new CacheException(ex);
        }
    }

    public async Task InvalidateCacheAsync(IDatabase database, params string[] keys)
    {
        try
        {
            foreach (var key in keys)
            {
                await database.KeyDeleteAsync(key);
                logger.LogInformation("Cache invalidated for key: {Key}", key);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while invalidating the cache: {Message}", ex.Message);
            throw new CacheException(ex);
        }
    }

    private sealed class PrivateResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);

            // TODO: is there any point checking if i know for sure that it's true
            // if (!prop.Writable)
            // {
            //     var property = member as PropertyInfo;
            //
            //     var hasPrivateSetter = property?.GetSetMethod() != null;
            //
            //     prop.Writable = hasPrivateSetter;
            // }

            prop.Writable = true;

            return prop;
        }
    }
}