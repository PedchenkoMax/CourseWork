using System.Reflection;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

namespace Catalog.Infrastructure.Cache.Extensions;

using Newtonsoft.Json;

public static class RedisCacheExtensions
{
    private static JsonSerializerSettings JsonSettings { get; } = new()
    {
        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
        ContractResolver = new PrivateResolver()
    };

    public static async Task<T> GetFromCacheAsync<T>(this IDatabase database, string key, Func<Task<T>> fetchFromDb, TimeSpan? expiry = null)
    {
        var value = await database.StringGetAsync(key);

        if (!value.IsNull)
        {
            return JsonConvert.DeserializeObject<T>(value, JsonSettings);
        }

        var result = await fetchFromDb();

        await database.StringSetAsync(key, JsonConvert.SerializeObject(result, JsonSettings), expiry);

        return result;
    }

    public static async Task InvalidateCacheAsync(this IDatabase database, params string[] keys)
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