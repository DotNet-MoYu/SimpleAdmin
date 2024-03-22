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
/// 系统配置服务
/// </summary>
public interface IConfigService : ITransient
{
    #region 查询

    /// <summary>
    /// 根据分类和配置键获配置
    /// </summary>
    /// <param name="category">分类</param>
    /// <param name="configKey">配置键</param>
    /// <returns>配置信息</returns>
    Task<SysConfig> GetByConfigKey(string category, string configKey);

    /// <summary>
    /// 根据分类获取配置列表
    /// </summary>
    /// <param name="category">分类名称</param>
    /// <returns>配置列表</returns>
    Task<List<SysConfig>> GetConfigsByCategory(string category);

    /// <summary>
    /// 分页查询其他配置
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>其他配置列表</returns>
    Task<SqlSugarPagedList<SysConfig>> Page(ConfigPageInput input);

    Task<FileStreamResult> GetIco();

    /// <summary>
    /// 获取系统配置列表,不包含业务的
    /// </summary>
    /// <returns></returns>
    Task<List<SysConfig>> GetSysConfigList();

    /// <summary>
    /// 是否是多租户模式
    /// </summary>
    /// <returns></returns>
    Task<bool> IsTenant();

    #endregion

    #region 新增

    /// <summary>
    /// 新增配置
    /// </summary>
    /// <param name="input">新增参数</param>
    /// <returns></returns>
    Task Add(ConfigAddInput input);

    #endregion

    #region 编辑

    /// <summary>
    /// 修改配置
    /// </summary>
    /// <param name="input">修改参数</param>
    /// <returns></returns>
    Task Edit(ConfigEditInput input);

    /// <summary>
    /// 批量编辑
    /// </summary>
    /// <param name="devConfigs">配置列表</param>
    /// <returns></returns>
    Task EditBatch(List<SysConfig> devConfigs);

    #endregion

    #region 删除

    /// <summary>
    /// 删除配置
    /// </summary>
    /// <param name="input">删除</param>
    /// <returns></returns>
    Task Delete(ConfigDeleteInput input);

    #endregion
}
