namespace SimpleAdmin.System;

/// <summary>
/// 角色
///</summary>
[SugarTable("sys_role", TableDescription = "角色")]
[Tenant(SqlsugarConst.DB_Default)]
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
    [SugarColumn(ColumnName = "DefaultDataScope", ColumnDescription = "默认数据范围", IsJson = true, ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = false)]
    public virtual DefaultDataScope DefaultDataScope { get; set; }

    /// <summary>
    /// 排序码
    ///</summary>
    [SugarColumn(ColumnName = "SortCode", ColumnDescription = "排序码", IsNullable = true)]
    public int? SortCode { get; set; }
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