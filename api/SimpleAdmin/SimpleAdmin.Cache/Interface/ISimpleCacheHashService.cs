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
/// 缓存服务
/// </summary>
public partial interface ISimpleCacheService
{
    /// <summary>
    /// 添加一条数据到HashMap
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">键</param>
    /// <param name="hashKey">hash列表里的Key</param>
    /// <param name="value">值</param>
    void HashAdd<T>(string key, string hashKey, T value);

    /// <summary>
    /// 添加多条数据到HashMap
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">键</param>
    /// <param name="dic">键值对字典</param>
    /// <returns></returns>
    bool HashSet<T>(string key, Dictionary<string, T> dic);

    /// <summary>
    /// 从HashMap中删除数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">键</param>
    /// <param name="fields">hash键列表</param>
    /// <returns>执行结果</returns>
    int HashDel<T>(string key, params string[] fields);

    /// <summary>
    /// 根据键获取hash列表中的值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">键</param>
    /// <param name="fields">hash键列表</param>
    /// <returns>数据列表</returns>
    List<T> HashGet<T>(string key, params string[] fields);

    /// <summary>
    /// 根据键获取hash列表中的值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">键</param>
    /// <param name="field">hash键</param>
    /// <returns></returns>
    T HashGetOne<T>(string key, string field);

    /// <summary>
    /// 获取所有键值对
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">键</param>
    /// <returns>数据字典</returns>
    IDictionary<string, T> HashGetAll<T>(string key);
}
