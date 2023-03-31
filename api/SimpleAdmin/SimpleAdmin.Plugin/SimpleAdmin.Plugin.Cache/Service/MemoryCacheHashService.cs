using Masuit.Tools;

namespace SimpleAdmin.Plugin.Cache;

/// <summary>
/// <inheritdoc cref="ISimpleCacheService"/>
/// 内存缓存
/// </summary>
public partial class MemoryCacheService : ISimpleCacheService
{

    /// <inheritdoc/>
    public void HashAdd<T>(string key, string hashKey, T value)
    {
        //获取字典
        var exist = _memoryCache.GetDictionary<T>(key);
        if (exist.ContainsKey(hashKey))//如果包含Key
            exist[hashKey] = value;//重新赋值
        else exist.Add(hashKey, value);//加上新的值
        _memoryCache.Set(key, exist);
    }

    //private IDictionary<string,T> GetDictionary(string key,string)

    /// <inheritdoc/>
    public bool HashSet<T>(string key, Dictionary<string, T> dic)
    {
        //获取字典
        var exist = _memoryCache.GetDictionary<T>(key);
        dic.ForEach(it =>
        {
            if (exist.ContainsKey(it.Key))//如果包含Key
                exist[it.Key] = it.Value;//重新赋值
            else exist.Add(it.Key, it.Value);//加上新的值
        });

        return true;
    }

    /// <inheritdoc/>
    public int HashDel<T>(string key, params string[] fields)
    {
        int result = 0;
        //获取字典
        var exist = _memoryCache.GetDictionary<T>(key);
        foreach (var field in fields)
        {
            if (exist.ContainsKey(field))//如果包含Key
            {
                exist.Remove(field);//删除
                result++;
            }
        }
        return result;
    }

    /// <inheritdoc/>
    public List<T> HashGet<T>(string key, params string[] fields)
    {
        List<T> list = new List<T>();
        //获取字典
        var exist = _memoryCache.GetDictionary<T>(key);
        foreach (var field in fields)
        {
            if (exist.ContainsKey(field))//如果包含Key
            {
                list.Add(exist[field]);
            }
            else { list.Add(default); }
        }
        return list;
    }

    /// <inheritdoc/>
    public T HashGetOne<T>(string key, string field)
    {
        //获取字典
        var exist = _memoryCache.GetDictionary<T>(key);

        exist.TryGetValue(field, out T result);
        return result;
    }

    /// <inheritdoc/>
    public IDictionary<string, T> HashGetAll<T>(string key)
    {
        return _memoryCache.GetDictionary<T>(key);
    }



}
