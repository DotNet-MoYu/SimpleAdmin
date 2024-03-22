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
/// 操作日志表
///</summary>
[SugarTable("sys_log_operate_{year}{month}{day}", TableDescription = "操作日志表")]
[Tenant(SqlSugarConst.DB_DEFAULT)]
public class SysLogOperate : SysLogVisit
{
    /// <summary>
    /// 具体消息
    ///</summary>
    [SugarColumn(ColumnName = "ExeMessage", ColumnDescription = "具体消息", ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
    public string ExeMessage { get; set; }

    /// <summary>
    /// 类名称
    ///</summary>
    [SugarColumn(ColumnName = "ClassName", ColumnDescription = "类名称", Length = 200)]
    public string ClassName { get; set; }

    /// <summary>
    /// 方法名称
    ///</summary>
    [SugarColumn(ColumnName = "MethodName", ColumnDescription = "方法名称", Length = 200)]
    public string MethodName { get; set; }

    /// <summary>
    /// 请求方式
    ///</summary>
    [SugarColumn(ColumnName = "ReqMethod", ColumnDescription = "请求方式", Length = 200, IsNullable = true)]
    public string ReqMethod { get; set; }

    /// <summary>
    /// 请求地址
    ///</summary>
    [SugarColumn(ColumnName = "ReqUrl", ColumnDescription = "请求地址", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string ReqUrl { get; set; }

    /// <summary>
    /// 请求参数
    ///</summary>
    [SugarColumn(ColumnName = "ParamJson", ColumnDescription = "请求参数", ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
    public string ParamJson { get; set; }

    /// <summary>
    /// 返回结果
    ///</summary>
    [SugarColumn(ColumnName = "ResultJson", ColumnDescription = "返回结果", ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
    public string ResultJson { get; set; }
}
