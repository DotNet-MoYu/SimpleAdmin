namespace SimpleAdmin.Plugin.Cache;

/// <summary>
/// <inheritdoc cref="ISimpleCacheService"/>
/// Redis缓存
/// </summary>
public partial class RedisCacheService : ISimpleCacheService
{
    private readonly ISimpleRedis _simpleRedis;

    public RedisCacheService(ISimpleRedis simpleRedis)
    {

        this._simpleRedis = simpleRedis;
    }

    #region 普通操作

    /// <inheritdoc/>
    public T Get<T>(string key)
    {
        return _simpleRedis.Get<T>(key);
    }


    /// <inheritdoc/>
    public int Remove(params string[] keys)
    {
        return _simpleRedis.GetFullRedis().Remove(keys);
    }



    /// <inheritdoc/>
    public bool Set<T>(string key, T value, int expire = -1)
    {
        return _simpleRedis.Set(key, value, expire);
    }

    /// <inheritdoc/>
    public bool Set<T>(string key, T value, TimeSpan expire)
    {
        return _simpleRedis.Set(key, value, expire);
    }


    /// <inheritdoc/>
    public bool SetExpire(string key, TimeSpan expire)
    {
        return _simpleRedis.GetFullRedis().SetExpire(key, expire);
    }



    /// <inheritdoc/>
    public TimeSpan GetExpire(string key)
    {
        return _simpleRedis.GetFullRedis().GetExpire(key);
    }


    /// <inheritdoc/>
    public bool ContainsKey(string key)
    {
        return _simpleRedis.ContainsKey(key);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _simpleRedis.Clear();
    }

    /// <inheritdoc/>
    public void DelByPattern(string pattern)
    {
        _simpleRedis.DelByPattern(pattern);
    }
    #endregion

    #region 集合操作

    /// <inheritdoc/>
    public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
    {
        return _simpleRedis.GetFullRedis().GetAll<T>(keys);
    }

    /// <inheritdoc/>
    public void SetAll<T>(IDictionary<string, T> values, int expire = -1)
    {
        _simpleRedis.GetFullRedis().SetAll(values, expire);
    }

    /// <inheritdoc/>
    public IDictionary<string, T> GetDictionary<T>(string key)
    {
        return _simpleRedis.GetFullRedis().GetDictionary<T>(key);
    }

    /// <inheritdoc/>
    public IProducerConsumer<T> GetQueue<T>(string key)
    {
        return _simpleRedis.GetFullRedis().GetQueue<T>(key);
    }

    /// <inheritdoc/>
    public IProducerConsumer<T> GetStack<T>(string key)
    {
        return _simpleRedis.GetFullRedis().GetStack<T>(key);
    }

    /// <inheritdoc/>
    public ICollection<T> GetSet<T>(string key)
    {
        return _simpleRedis.GetFullRedis().GetSet<T>(key);
    }
    #endregion

    #region 高级操作

    /// <inheritdoc/>
    public bool Add<T>(string key, T value, int expire = -1)
    {
        return _simpleRedis.GetFullRedis().Add(key, value, expire);
    }

    /// <inheritdoc/>
    public IList<T> GetList<T>(string key)
    {
        return _simpleRedis.GetFullRedis().GetList<T>(key);
    }

    /// <inheritdoc/>
    public T Replace<T>(string key, T value)
    {
        return _simpleRedis.GetFullRedis().Replace(key, value);
    }

    /// <inheritdoc/>
    public T GetOrAdd<T>(string key, Func<string, T> callback, int expire = -1)
    {
        return _simpleRedis.GetFullRedis().GetOrAdd(key, callback, expire);
    }

    /// <inheritdoc/>
    public bool TryGetValue<T>(string key, out T value)
    {
        return _simpleRedis.GetFullRedis().TryGetValue(key, out value);
    }

    /// <inheritdoc/>
    public long Decrement(string key, long value)
    {
        return _simpleRedis.GetFullRedis().Decrement(key, value);
    }

    /// <inheritdoc/>
    public double Decrement(string key, double value)
    {
        return _simpleRedis.GetFullRedis().Decrement(key, value);
    }


    /// <inheritdoc/>
    public long Increment(string key, long value)
    {
        return _simpleRedis.GetFullRedis().Increment(key, value);
    }

    /// <inheritdoc/>
    public double Increment(string key, double value)
    {
        return _simpleRedis.GetFullRedis().Increment(key, value);
    }
    #endregion

    #region 事务

    /// <inheritdoc/>
    public int Commit()
    {
        return _simpleRedis.GetFullRedis().Commit();
    }

    /// <inheritdoc/>
    public IDisposable AcquireLock(string key, int msTimeout)
    {
        return _simpleRedis.GetFullRedis().AcquireLock(key, msTimeout);
    }

    /// <inheritdoc/>
    public IDisposable AcquireLock(string key, int msTimeout, int msExpire, bool throwOnFailure)
    {
        return _simpleRedis.GetFullRedis().AcquireLock(key, msTimeout, msExpire, throwOnFailure);
    }
    #endregion

}
