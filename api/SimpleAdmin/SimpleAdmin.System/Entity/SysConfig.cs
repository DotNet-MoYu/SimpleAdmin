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
/// 配置
///</summary>
[SugarTable("sys_config", TableDescription = "配置")]
[Tenant(SqlSugarConst.DB_DEFAULT)]
public class SysConfig : BaseEntity
{
    /// <summary>
    /// 配置键
    ///</summary>
    [SugarColumn(ColumnName = "ConfigKey", ColumnDescription = "配置键", Length = 200)]
    public virtual string ConfigKey { get; set; }

    /// <summary>
    /// 配置值
    ///</summary>
    [SugarColumn(ColumnName = "ConfigValue", ColumnDescription = "配置值", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public virtual string ConfigValue { get; set; }

    /// <summary>
    /// 分类
    ///</summary>
    [SugarColumn(ColumnName = "Category", ColumnDescription = "分类", Length = 200)]
    public string Category { get; set; }

    /// <summary>
    /// 备注
    ///</summary>
    [SugarColumn(ColumnName = "Remark", ColumnDescription = "备注", Length = 200, IsNullable = true)]
    public string Remark { get; set; }

    /// <summary>
    /// 排序码
    ///</summary>
    [SugarColumn(ColumnName = "SortCode", ColumnDescription = "排序码", IsNullable = true)]
    public int SortCode { get; set; }
}
