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
/// 操作日志服务
/// </summary>
public interface IOperateLogService : ITransient
{
    /// <summary>
    /// 根据分类删除操作日志
    /// </summary>
    /// <param name="category">分类名称</param>
    /// <returns></returns>
    Task Delete(string category);

    /// <summary>
    /// 日志详情
    /// </summary>
    /// <param name="input">id</param>
    /// <returns>日志详情</returns>
    Task<SysLogOperate> Detail(BaseIdInput input);

    /// <summary>
    /// 操作日志分页查询
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>分页列表</returns>
    Task<SqlSugarPagedList<SysLogOperate>> Page(OperateLogPageInput input);

    /// <summary>
    /// 操作日志周统计
    /// </summary>
    /// <param name="day">天数</param>
    /// <returns>统计结果</returns>
    Task<List<OperateLogDayStatisticsOutput>> StatisticsByDay(int day);

    /// <summary>
    /// 操作日志总览
    /// </summary>
    /// <returns>异常日志和操作日志的数量</returns>
    Task<List<OperateLogTotalCountOutput>> TotalCount();
}
