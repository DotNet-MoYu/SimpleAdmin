namespace SimpleAdmin.Plugin.Cache;

/// <summary>
/// <inheritdoc cref="ISimpleCacheService"/>
/// Redis缓存
/// </summary>

public partial class RedisCacheService : ISimpleCacheService
{


    /// <inheritdoc/>
    public void HashAdd<T>(string key, string hashKey, T value)
    {
        _simpleRedis.HashAdd<T>(key, hashKey, value);
    }

    /// <inheritdoc/>
    public bool HashSet<T>(string key, Dictionary<string, T> dic)
    {
        return _simpleRedis.HashSet<T>(key, dic);
    }

    /// <inheritdoc/>
    public int HashDel<T>(string key, params string[] fields)
    {
        return _simpleRedis.HashDel<T>(key, fields);
    }

    /// <inheritdoc/>
    public List<T> HashGet<T>(string key, params string[] fields)
    {
        return _simpleRedis.HashGet<T>(key, fields);
    }

    /// <inheritdoc/>
    public T HashGetOne<T>(string key, string field)
    {
        return _simpleRedis.HashGetOne<T>(key, field);
    }

    /// <inheritdoc/>
    public IDictionary<string, T> HashGetAll<T>(string key)
    {
        return _simpleRedis.HashGetAll<T>(key);
    }

}
