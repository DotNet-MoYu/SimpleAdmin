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
/// 角色
///</summary>
[SugarTable("sys_role", TableDescription = "角色")]
[Tenant(SqlSugarConst.DB_DEFAULT)]
public class SysRole : DataEntityBase
{
    /// <summary>
    /// 组织id
    ///</summary>
    [SugarColumn(ColumnName = "OrgId", ColumnDescription = "组织id", IsNullable = true)]
    public long? OrgId { get; set; }

    /// <summary>
    /// 名称
    ///</summary>
    [SugarColumn(ColumnName = "Name", ColumnDescription = "名称", Length = 200, IsNullable = false)]
    public virtual string Name { get; set; }

    /// <summary>
    /// 编码
    ///</summary>
    [SugarColumn(ColumnName = "Code", ColumnDescription = "编码", Length = 200, IsNullable = false)]
    public string Code { get; set; }

    /// <summary>
    /// 分类
    ///</summary>
    [SugarColumn(ColumnName = "Category", ColumnDescription = "分类", Length = 200, IsNullable = false)]
    public virtual string Category { get; set; }

    /// <summary>
    /// 默认数据范围
    ///</summary>
    [SugarColumn(ColumnName = "DefaultDataScope", ColumnDescription = "默认数据范围", IsJson = true, ColumnDataType = StaticConfig.CodeFirst_BigString,
        IsNullable = false)]
    public virtual DefaultDataScope DefaultDataScope { get; set; }

    /// <summary>
    /// 排序码
    ///</summary>
    [SugarColumn(ColumnName = "SortCode", ColumnDescription = "排序码", IsNullable = true)]
    public int? SortCode { get; set; }

    /// <summary>
    /// 用户列表
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<UserSelectorOutPut> UserList { get; set; } = new List<UserSelectorOutPut>();
}

/// <summary>
/// 默认数据范围
/// </summary>
public class DefaultDataScope
{
    /// <summary>
    /// 数据范围等级
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 数据范围
    /// </summary>
    public string ScopeCategory { get; set; }

    /// <summary>
    /// 自定义机构范围列表
    /// </summary>
    public List<long> ScopeDefineOrgIdList { get; set; } = new List<long>();
}
