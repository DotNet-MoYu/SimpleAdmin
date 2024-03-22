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
/// 访问日志服务
/// </summary>
public interface IVisitLogService : ITransient
{
    /// <summary>
    /// 根据分类删除
    /// </summary>
    /// <param name="category">分类名称</param>
    /// <returns></returns>
    Task Delete(string category);

    /// <summary>
    /// 访问日志分页查询
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>日志列表</returns>
    Task<SqlSugarPagedList<SysLogVisit>> Page(VisitLogPageInput input);

    /// <summary>
    /// 根统计N天来登录和登出数量
    /// </summary>
    /// <param name="day">天使</param>
    /// <returns>统计信息</returns>
    Task<List<VisitLogDayStatisticsOutput>> StatisticsByDay(int day);

    /// <summary>
    /// 统计登录登出总览
    /// </summary>
    /// <returns>登录和登出的数量</returns>
    Task<List<VisitLogTotalCountOutput>> TotalCount();
}
