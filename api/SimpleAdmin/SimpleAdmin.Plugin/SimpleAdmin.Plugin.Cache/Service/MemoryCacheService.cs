namespace SimpleAdmin.Plugin.Cache;

/// <summary>
/// <inheritdoc cref="ISimpleCacheService"/>
/// 内存缓存
/// </summary>
public partial class MemoryCacheService : ISimpleCacheService
{
    public readonly MemoryCache _memoryCache;
    public MemoryCacheService()
    {

        _memoryCache = new MemoryCache();
    }

    #region 普通操作
    /// <inheritdoc/>
    public T Get<T>(string key)
    {
        return _memoryCache.Get<T>(key);
    }


    /// <inheritdoc/>
    public int Remove(params string[] keys)
    {
        return _memoryCache.Remove(keys);
    }



    /// <inheritdoc/>
    public bool Set<T>(string key, T value, int expire = -1)
    {
        return _memoryCache.Set(key, value, expire);
    }

    /// <inheritdoc/>
    public bool Set<T>(string key, T value, TimeSpan expire)
    {
        return _memoryCache.Set(key, value, expire);
    }


    /// <inheritdoc/>
    public bool SetExpire(string key, TimeSpan expire)
    {
        return _memoryCache.SetExpire(key, expire);
    }



    /// <inheritdoc/>
    public TimeSpan GetExpire(string key)
    {
        return _memoryCache.GetExpire(key);
    }


    /// <inheritdoc/>
    public bool ContainsKey(string key)
    {
        return _memoryCache.ContainsKey(key);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _memoryCache.Clear();
    }

    /// <inheritdoc/>
    public void DelByPattern(string pattern)
    {
        var keys = _memoryCache.Keys.ToList();//获取所有key
        keys.ForEach(it =>
        {
            if (it.Contains(pattern))//如果匹配
                _memoryCache.Remove(pattern);

        });

    }
    #endregion

    #region 集合操作

    /// <inheritdoc/>
    public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
    {
        return _memoryCache.GetAll<T>(keys);
    }

    /// <inheritdoc/>
    public void SetAll<T>(IDictionary<string, T> values, int expire = -1)
    {
        _memoryCache.SetAll(values, expire);
    }

    /// <inheritdoc/>
    public IDictionary<string, T> GetDictionary<T>(string key)
    {
        return _memoryCache.GetDictionary<T>(key);
    }

    /// <inheritdoc/>
    public IProducerConsumer<T> GetQueue<T>(string key)
    {
        return _memoryCache.GetQueue<T>(key);
    }

    /// <inheritdoc/>
    public IProducerConsumer<T> GetStack<T>(string key)
    {
        return _memoryCache.GetStack<T>(key);
    }

    /// <inheritdoc/>
    public ICollection<T> GetSet<T>(string key)
    {
        return _memoryCache.GetSet<T>(key);
    }
    #endregion

    #region 高级操作

    /// <inheritdoc/>
    public bool Add<T>(string key, T value, int expire = -1)
    {
        return _memoryCache.Add(key, value, expire);
    }

    /// <inheritdoc/>
    public IList<T> GetList<T>(string key)
    {
        return _memoryCache.GetList<T>(key);
    }

    /// <inheritdoc/>
    public T Replace<T>(string key, T value)
    {
        return _memoryCache.Replace(key, value);
    }

    /// <inheritdoc/>
    public T GetOrAdd<T>(string key, Func<string, T> callback, int expire = -1)
    {
        return _memoryCache.GetOrAdd<T>(key, callback, expire);
    }

    /// <inheritdoc/>
    public bool TryGetValue<T>(string key, out T value)
    {
        return _memoryCache.TryGetValue(key, out value);
    }

    /// <inheritdoc/>
    public long Decrement(string key, long value)
    {
        return _memoryCache.Decrement(key, value);
    }

    /// <inheritdoc/>
    public double Decrement(string key, double value)
    {
        return _memoryCache.Decrement(key, value);
    }


    /// <inheritdoc/>
    public long Increment(string key, long value)
    {
        return _memoryCache.Increment(key, value);
    }

    /// <inheritdoc/>
    public double Increment(string key, double value)
    {
        return _memoryCache.Increment(key, value);
    }
    #endregion

    #region 事务

    /// <inheritdoc/>
    public int Commit()
    {
        return _memoryCache.Commit();
    }

    /// <inheritdoc/>
    public IDisposable AcquireLock(string key, int msTimeout)
    {
        return _memoryCache.AcquireLock(key, msTimeout);
    }

    /// <inheritdoc/>
    public IDisposable AcquireLock(string key, int msTimeout, int msExpire, bool throwOnFailure)
    {
        return _memoryCache.AcquireLock(key, msTimeout, msExpire, throwOnFailure);
    }
    #endregion
}
