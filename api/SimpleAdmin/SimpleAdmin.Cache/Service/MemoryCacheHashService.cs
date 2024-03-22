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
public partial class MemoryCacheService
{
    /// <inheritdoc/>
    public void HashAdd<T>(string key, string hashKey, T value)
    {
        //获取字典
        var exist = _memoryCache.GetDictionary<T>(key);
        if (exist.ContainsKey(hashKey))//如果包含Key
            exist[hashKey] = value;//重新赋值
        else exist.TryAdd(hashKey, value);//加上新的值
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
        var result = 0;
        //获取字典
        var exist = _memoryCache.GetDictionary<T>(key);
        foreach (var field in fields)
        {
            if (field != null && exist.ContainsKey(field))//如果包含Key
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
        var list = new List<T>();
        //获取字典
        var exist = _memoryCache.GetDictionary<T>(key);
        foreach (var field in fields)
        {
            if (exist.TryGetValue(field, out var value))//如果包含Key
            {
                list.Add(value);
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

        exist.TryGetValue(field, out var result);
        var data = result.DeepClone();
        return data;
    }

    /// <inheritdoc/>
    public IDictionary<string, T> HashGetAll<T>(string key)
    {
        var data = _memoryCache.GetDictionary<T>(key);
        return data;
    }
}
