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
/// 批量修改配置表
///</summary>
[SugarTable("batch_edit_config", TableDescription = "批量修改配置表")]
[Tenant(SqlSugarConst.DB_DEFAULT)]
public class BatchEditConfig : PrimaryKeyEntity
{
    /// <summary>
    /// 批量配置Id
    ///</summary>
    [SugarColumn(ColumnName = "UId", ColumnDescription = "批量配置Id", IsNullable = false)]
    public long UId { get; set; }

    /// <summary>
    /// 字段名
    ///</summary>
    [SugarColumn(ColumnName = "ColumnName", ColumnDescription = "字段名", Length = 100, IsNullable = false)]
    public string ColumnName { get; set; }

    /// <summary>
    /// 字段描述
    ///</summary>
    [SugarColumn(ColumnName = "ColumnComment", ColumnDescription = "字段描述", Length = 100, IsNullable = false)]
    public string ColumnComment { get; set; }

    /// <summary>
    /// 作用类型
    ///</summary>
    [SugarColumn(ColumnName = "DataType", ColumnDescription = "作用类型", Length = 100, IsNullable = false)]
    public string DataType { get; set; }

    /// <summary>
    /// 字典值
    ///</summary>
    [SugarColumn(ColumnName = "DictTypeCode", ColumnDescription = "字典值", Length = 100, IsNullable = true)]
    public string DictTypeCode { get; set; }

    /// <summary>
    /// 数据库类型
    ///</summary>
    [SugarColumn(ColumnName = "NetType", ColumnDescription = "数据库类型", Length = 100, IsNullable = true)]
    public string NetType { get; set; }

    /// <summary>
    /// 接口名称
    /// </summary>
    [SugarColumn(ColumnName = "RequestUrl", ColumnDescription = "接口名称", Length = 100, IsNullable = true)]
    public string RequestUrl { get; set; }

    /// <summary>
    /// 接口类型
    /// </summary>
    [SugarColumn(ColumnName = "RequestType", ColumnDescription = "接口类型", Length = 100, IsNullable = true)]
    public string RequestType { get; set; }

    /// <summary>
    /// 接口结果标签
    /// </summary>
    [SugarColumn(ColumnName = "RequestLabel", ColumnDescription = "接口结果标签", Length = 100, IsNullable = true)]
    public string RequestLabel { get; set; }

    /// <summary>
    /// 接口结果值
    /// </summary>
    [SugarColumn(ColumnName = "RequestValue", ColumnDescription = "接口结果值", Length = 100, IsNullable = true)]
    public string RequestValue { get; set; }

    /// <summary>
    /// 启用状态
    /// </summary>
    [SugarColumn(ColumnName = "Status", ColumnDescription = "状态", Length = 100, IsNullable = false)]
    public string Status { get; set; }
}
