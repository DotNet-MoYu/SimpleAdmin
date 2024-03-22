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
/// 组织
///</summary>
[SugarTable("sys_org", TableDescription = "组织")]
[Tenant(SqlSugarConst.DB_DEFAULT)]
[CodeGen]
public class SysOrg : BaseEntity
{
    /// <summary>
    /// 父id
    ///</summary>
    [SugarColumn(ColumnName = "ParentId", ColumnDescription = "父id")]
    public long ParentId { get; set; }

    [SugarColumn(ColumnName = "ParentIdList", ColumnDescription = "父id列表", IsNullable = true, IsJson = true)]
    public List<long> ParentIdList { get; set; } = new List<long>();

    /// <summary>
    /// 主管ID
    ///</summary>
    [SugarColumn(ColumnName = "DirectorId", ColumnDescription = "主管ID", IsNullable = true)]
    public long? DirectorId { get; set; }

    /// <summary>
    /// 名称
    ///</summary>
    [SugarColumn(ColumnName = "Name", ColumnDescription = "名称", Length = 200)]
    public string Name { get; set; }

    /// <summary>
    /// 全称
    ///</summary>
    [SugarColumn(ColumnName = "Names", ColumnDescription = "全称", Length = 500)]
    public string Names { get; set; }

    /// <summary>
    /// 编码
    ///</summary>
    [SugarColumn(ColumnName = "Code", ColumnDescription = "编码", Length = 200)]
    public string Code { get; set; }

    /// <summary>
    /// 分类
    ///</summary>
    [SugarColumn(ColumnName = "Category", ColumnDescription = "分类", Length = 200)]
    public string Category { get; set; }

    /// <summary>
    /// 排序码
    ///</summary>
    [SugarColumn(ColumnName = "SortCode", ColumnDescription = "排序码", IsNullable = true)]
    public int? SortCode { get; set; }

    /// <summary>
    /// 主管信息
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public UserSelectorOutPut DirectorInfo { get; set; }

    /// <summary>
    /// 子节点
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<SysOrg> Children { get; set; }

    /// <summary>
    /// 设置为叶子节点(设置了loadData时有效)
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public bool? Leaf { get; set; }
}
