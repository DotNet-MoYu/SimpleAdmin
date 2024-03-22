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
/// Redis缓存
/// </summary>
public partial class RedisCacheService : ISimpleCacheService
{
    private readonly ISimpleRedis _simpleRedis;

    public RedisCacheService(ISimpleRedis simpleRedis)
    {
        _simpleRedis = simpleRedis;
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

    #endregion 普通操作

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

    #endregion 集合操作

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

    #endregion 高级操作

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
    public IDisposable AcquireLock(string key, int msTimeout, int msExpire,
        bool throwOnFailure)
    {
        return _simpleRedis.GetFullRedis().AcquireLock(key, msTimeout, msExpire, throwOnFailure);
    }

    #endregion 事务
}
