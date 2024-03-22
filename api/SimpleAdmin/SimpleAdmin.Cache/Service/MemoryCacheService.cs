// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.Cache;

/// <summary>
/// <inheritdoc cref="ISimpleCacheService"/>
/// 内存缓存
/// </summary>
public partial class MemoryCacheService : ISimpleCacheService
{
    private readonly MemoryCache _memoryCache;

    public MemoryCacheService()
    {
        _memoryCache = new MemoryCache();
    }

    #region 普通操作

    /// <inheritdoc/>
    public T Get<T>(string key)
    {
        var data = _memoryCache.Get<string>(key);
        return data.ToObject<T>();
    }

    /// <inheritdoc/>
    public int Remove(params string[] keys)
    {
        return _memoryCache.Remove(keys);
    }

    /// <inheritdoc/>
    public bool Set<T>(string key, T value, int expire = -1)
    {
        return _memoryCache.Set(key, value.ToJson(), expire);
    }

    /// <inheritdoc/>
    public bool Set<T>(string key, T value, TimeSpan expire)
    {
        return _memoryCache.Set(key, value.ToJson(), expire);
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

    #endregion 普通操作

    #region 集合操作

    /// <inheritdoc/>
    public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
    {
        IDictionary<string, T>? result = default;//定义集合
        var data = _memoryCache.GetAll<string>(keys);//获取数据
        data.ForEach(it =>
        {
            result.Add(it.Key, it.Value.ToObject<T>());//遍历数据,格式化并加到新的数据集合
        });
        return result;
    }

    /// <inheritdoc/>
    public void SetAll<T>(IDictionary<string, T> values, int expire = -1)
    {
        IDictionary<string, string>? result = default;//定义集合
        values.ForEach(it =>
        {
            result.Add(it.Key, it.Value.ToJson());//遍历数据,格式化并加到新的数据集合
        });
        _memoryCache.SetAll(values, expire);
    }

    /// <inheritdoc/>
    public IDictionary<string, T> GetDictionary<T>(string key)
    {
        IDictionary<string, T>? result = default;//定义集合
        var data = _memoryCache.GetDictionary<string>(key);
        data.ForEach(it =>
        {
            result.Add(it.Key, it.Value.ToObject<T>());//遍历数据,格式化并加到新的数据集合
        });
        return result;
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

    #endregion 集合操作

    #region 高级操作

    /// <inheritdoc/>
    public bool Add<T>(string key, T value, int expire = -1)
    {
        return _memoryCache.Add(key, value.ToJson(), expire);
    }

    /// <inheritdoc/>
    public IList<T> GetList<T>(string key)
    {
        IList<T> result = default;//定义集合
        var data = _memoryCache.GetList<string>(key);
        data.ForEach(it =>
        {
            result.Add(it.ToObject<T>());//遍历数据,格式化并加到新的数据集合
        });
        return result;
    }

    /// <inheritdoc/>
    public T Replace<T>(string key, T value)
    {
        return _memoryCache.Replace(key, value);
    }

    /// <inheritdoc/>
    public bool TryGetValue<T>(string key, out T value)
    {
        _ = _memoryCache.TryGetValue<string>(key, out var result);
        value = result.ToObject<T>();
        return value == null;
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

    #endregion 高级操作

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
    public IDisposable AcquireLock(string key, int msTimeout, int msExpire,
        bool throwOnFailure)
    {
        return _memoryCache.AcquireLock(key, msTimeout, msExpire, throwOnFailure);
    }

    #endregion 事务
}
