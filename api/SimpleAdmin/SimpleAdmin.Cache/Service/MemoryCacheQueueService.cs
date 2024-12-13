﻿// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
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
/// <inheritdoc cref="ISimpleCacheService"/>
/// 内存缓存
/// </summary>
public partial class MemoryCacheService
{
    /// <inheritdoc/>
    public int AddQueue<T>(string key, T[] value)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public int AddQueue<T>(string key, T value)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public RedisQueue<T> GetRedisQueue<T>(string key)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public List<T> GetQueue<T>(string key, int Count = 1)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public T GetQueueOne<T>(string key, int timeout = 1)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task<T> GetQueueOneAsync<T>(string key, int timeout = 1)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public int AddDelayQueue<T>(string key, T value, int delay)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public int AddDelayQueue<T>(string key, List<T> value, int delay)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public RedisDelayQueue<T> GetDelayQueue<T>(string key)
    {
        throw new NotImplementedException();
    }
}
