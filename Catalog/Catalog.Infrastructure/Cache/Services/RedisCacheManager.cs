using System.Reflection;
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
        var value = await database.StringGetAsync(key);

        if (!value.IsNull)
            return JsonConvert.DeserializeObject<T>(value, jsonSettings);

        var result = await fetchFromDb();

        await database.StringSetAsync(key, JsonConvert.SerializeObject(result, jsonSettings), expiry);

        return result;
    }

    public async Task InvalidateCacheAsync(IDatabase database, params string[] keys)
    {
        foreach (var key in keys)
        {
            await database.KeyDeleteAsync(key);
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