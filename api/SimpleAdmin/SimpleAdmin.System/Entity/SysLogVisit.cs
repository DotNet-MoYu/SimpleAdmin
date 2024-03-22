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
/// 访问日志表
///</summary>
[SugarTable("sys_log_visit_{year}{month}{day}", TableDescription = "访问日志表")]
[SplitTable(SplitType.Year)]//按年分表 （自带分表支持 年、季、月、周、日）
[Tenant(SqlSugarConst.DB_DEFAULT)]
public class SysLogVisit : BaseEntity
{
    /// <summary>
    /// 日志分类
    ///</summary>
    [SugarColumn(ColumnName = "Category", ColumnDescription = "日志分类", Length = 200)]
    public string Category { get; set; }

    /// <summary>
    /// 日志名称
    ///</summary>
    [SugarColumn(ColumnName = "Name", ColumnDescription = "日志名称", Length = 200)]
    public string Name { get; set; }

    /// <summary>
    /// 执行状态
    ///</summary>
    [SugarColumn(ColumnName = "ExeStatus", ColumnDescription = "执行状态", Length = 200)]
    public string ExeStatus { get; set; }

    /// <summary>
    /// 操作ip
    ///</summary>
    [SugarColumn(ColumnName = "OpIp", ColumnDescription = "操作ip", Length = 200)]
    public string OpIp { get; set; }

    /// <summary>
    /// 操作地址
    ///</summary>
    [SugarColumn(ColumnName = "OpAddress", ColumnDescription = "操作地址", Length = 200)]
    public string OpAddress { get; set; }

    /// <summary>
    /// 操作浏览器
    ///</summary>
    [SugarColumn(ColumnName = "OpBrowser", ColumnDescription = "操作浏览器", Length = 200)]
    public string OpBrowser { get; set; }

    /// <summary>
    /// 操作系统
    ///</summary>
    [SugarColumn(ColumnName = "OpOs", ColumnDescription = "操作系统", Length = 200)]
    public string OpOs { get; set; }

    /// <summary>
    /// 操作时间
    ///</summary>
    [SugarColumn(ColumnName = "OpTime", ColumnDescription = "操作时间")]
    [SplitField]//分表字段 在插入的时候会根据这个字段插入哪个表，在更新删除的时候用这个字段找出相关表
    public DateTime OpTime { get; set; }

    /// <summary>
    /// 操作人姓名
    ///</summary>
    [SugarColumn(ColumnName = "OpUser", ColumnDescription = "操作人姓名", Length = 200, IsNullable = true)]
    public string OpUser { get; set; }

    /// <summary>
    /// 操作人姓名
    ///</summary>
    [SugarColumn(ColumnName = "OpAccount", ColumnDescription = "操作人账号", Length = 200, IsNullable = true)]
    public string OpAccount { get; set; }
}
