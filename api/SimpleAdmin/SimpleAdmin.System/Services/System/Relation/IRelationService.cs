// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.System;

/// <summary>
/// 关系服务
/// </summary>
public interface IRelationService : ITransient
{
    /// <summary>
    /// 根据分类获取关系表信息
    /// </summary>
    /// <param name="category">分类名称</param>
    /// <returns>关系表</returns>
    Task<List<SysRelation>> GetRelationByCategory(string category);

    /// <summary>
    /// 通过对象ID和分类获取关系列表
    /// </summary>
    /// <param name="objectId">对象ID</param>
    /// <param name="category">分类</param>
    /// <returns></returns>
    Task<List<SysRelation>> GetRelationListByObjectIdAndCategory(long objectId, string category);

    /// <summary>
    /// 通过对象ID列表和分类获取关系列表
    /// </summary>
    /// <param name="objectIds">对象ID</param>
    /// <param name="category">分类</param>
    /// <returns></returns>
    Task<List<SysRelation>> GetRelationListByObjectIdListAndCategory(List<long> objectIds, string category);

    /// <summary>
    /// 通过目标ID和分类获取关系列表
    /// </summary>
    /// <param name="targetId">目标ID</param>
    /// <param name="category">分类</param>
    /// <returns></returns>
    Task<List<SysRelation>> GetRelationListByTargetIdAndCategory(string targetId, string category);

    /// <summary>
    /// 通过目标ID列表和分类获取关系列表
    /// </summary>
    /// <param name="targetIds"></param>
    /// <param name="category"></param>
    /// <returns></returns>
    Task<List<SysRelation>> GetRelationListByTargetIdListAndCategory(List<string> targetIds, string category);

    /// <summary>
    /// 获取关系表用户工作台
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>关系表数据</returns>
    Task<SysRelation> GetWorkbench(long userId);

    /// <summary>
    /// 更新缓存
    /// </summary>
    /// <param name="category">分类</param>
    /// <returns></returns>
    Task RefreshCache(string category);

    /// <summary>
    /// 保存关系
    /// </summary>
    /// <param name="category">分类</param>
    /// <param name="objectId">对象ID</param>
    /// <param name="targetId">目标ID</param>
    /// <param name="extJson">拓展信息</param>
    /// <param name="clear">是否清除老的数据</param>
    /// <param name="refreshCache">是否刷新缓存</param>
    /// <returns></returns>
    Task SaveRelation(string category, long objectId, string targetId,
        string extJson, bool clear, bool refreshCache = true);

    /// <summary>
    /// 批量保存关系
    /// </summary>
    /// <param name="category">分类</param>
    /// <param name="objectId">对象ID</param>
    /// <param name="targetIds">目标ID列表</param>
    /// <param name="extJsons">拓展信息列表</param>
    /// <param name="clear">是否清除老的数据</param>
    /// <returns></returns>
    Task SaveRelationBatch(string category, long objectId, List<string> targetIds,
        List<string> extJsons, bool clear);

    /// <summary>
    /// 获取用户模块ID
    /// </summary>
    /// <param name="roleIdList"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<List<long>> GetUserModuleId(List<long> roleIdList, long userId);
}
