// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

using NewLife.Caching.Queues;

namespace SimpleAdmin.Cache;

/// <summary>
/// 缓存服务
/// </summary>
public partial interface ISimpleCacheService
{
    #region 普通队列

    /// <summary>
    /// 添加到队列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">键</param>
    /// <param name="value">值数组</param>
    /// <returns>添加数量</returns>
    int AddQueue<T>(string key, T[] value);

    /// <summary>
    /// 添加到队列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>添加数量</returns>
    int AddQueue<T>(string key, T value);

    /// <summary>
    /// 获取队列实例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">键</param>
    /// <returns>队列实例</returns>
    RedisQueue<T> GetRedisQueue<T>(string key);

    /// <summary>
    /// 从队列中获取数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">键</param>
    /// <param name="Count">数量</param>
    /// <returns>数据列表</returns>
    List<T> GetQueue<T>(string key, int Count = 1);

    /// <summary>
    /// 取一条数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">键</param>
    /// <param name="timeout">阻塞时间</param>
    /// <returns>数据</returns>
    T GetQueueOne<T>(string key, int timeout = 1);

    /// <summary>
    /// 异步取一条数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">键</param>
    /// <param name="timeout">阻塞时间</param>
    /// <returns>数据</returns>
    Task<T> GetQueueOneAsync<T>(string key, int timeout = 1);

    #endregion

    #region 延迟队列

    /// <summary>
    /// 添加一条数据到延迟队列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="delay">延迟时间</param>
    /// <returns>添加成功数量</returns>
    int AddDelayQueue<T>(string key, T value, int delay);

    /// <summary>
    /// 批量添加数据到延迟队列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="delay">延迟时间</param>
    /// <returns>添加成功数量</returns>
    int AddDelayQueue<T>(string key, List<T> value, int delay);

    /// <summary>
    /// 获取延迟队列实例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">键</param>
    /// <returns></returns>
    RedisDelayQueue<T> GetDelayQueue<T>(string key);

    #endregion
}
