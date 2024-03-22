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
/// 批量服务
/// </summary>
public interface IBatchEditService : ITransient
{
    /// <summary>
    /// 批量分页查询
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>分页结果</returns>
    Task<SqlSugarPagedList<BatchEdit>> Page(BatchEditPageInput input);

    /// <summary>
    /// 添加批量
    /// </summary>
    /// <param name="input">添加参数</param>
    /// <returns></returns>
    Task Add(BatchEditAddInput input);

    /// <summary>
    /// 删除批量
    /// </summary>
    /// <param name="input">删除参数</param>
    /// <returns></returns>
    Task Delete(BaseIdListInput input);

    /// <summary>
    /// 获取需要批量修改的表
    /// </summary>
    /// <returns></returns>
    List<SqlSugarTableInfo> GetTables();

    /// <summary>
    /// 获取批量修改配置
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<List<BatchEditConfig>> ConfigList(BaseIdInput input);

    /// <summary>
    /// 配置字段
    /// </summary>
    /// <param name="input">字段信息</param>
    /// <returns></returns>
    Task Config(List<BatchEditConfigInput> input);

    /// <summary>
    /// 批量配置字段列表
    /// </summary>
    /// <param name="code">唯一编码</param>
    /// <returns>列表</returns>
    Task<List<BatchEditConfig>> Columns(string code);

    /// <summary>
    /// 获取字典配置
    /// </summary>
    /// <param name="code">唯一编码</param>
    /// <param name="columns">字段信息</param>
    /// <returns>SqlSugar对应字典</returns>
    Task<Dictionary<string, object>> GetUpdateBatchConfigDict(string code, List<BatchEditColumn> columns);

    /// <summary>
    /// 同步字段
    /// </summary>
    /// <param name="input">id</param>
    /// <returns></returns>
    Task SyncColumns(BaseIdInput input);
}
