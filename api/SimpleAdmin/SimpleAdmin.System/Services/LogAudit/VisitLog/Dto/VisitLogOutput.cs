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
/// 周统计输出
/// </summary>
public class VisitLogDayStatisticsOutput
{
    /// <summary>
    /// 日期
    /// </summary>
    public string Date { get; set; }

    /// <summary>
    /// 登录次数
    /// </summary>
    public int LoginCount { get; set; }

    /// <summary>
    /// 登出次数
    /// </summary>
    public int LogoutCount { get; set; }
}

/// <summary>
/// 访问日志总数输出
/// </summary>
public class VisitLogTotalCountOutput
{
    /// <summary>
    /// 类型
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    public int Value { get; set; }
}
