namespace SimpleAdmin.Plugin.SqlSugar;

/// <summary>
/// 组织
///</summary>
[SugarTable("sys_org", TableDescription = "组织")]
[Tenant(SqlsugarConst.DB_Default)]
[CodeGen]
public class SysOrg : BaseEntity
{
    /// <summary>
    /// 父id
    ///</summary>
    [SugarColumn(ColumnName = "ParentId", ColumnDescription = "父id")]
    public long ParentId { get; set; }

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
    /// 子节点
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<SysOrg> Children { get; set; }
}