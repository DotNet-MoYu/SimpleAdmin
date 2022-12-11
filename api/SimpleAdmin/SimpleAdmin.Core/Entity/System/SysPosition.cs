namespace SimpleAdmin.Core;

/// <summary>
/// 职位表
///</summary>
[SugarTable("sys_position", TableDescription = "职位表")]
[Tenant(SqlsugarConst.DB_Default)]
public class SysPosition : DataEntityBase
{
    /// <summary>
    /// 组织id 
    ///</summary>
    [SugarColumn(ColumnName = "OrgId", ColumnDescription = "组织id")]
    public virtual long OrgId { get; set; }
    /// <summary>
    /// 名称 
    ///</summary>
    [SugarColumn(ColumnName = "Name", ColumnDescription = "名称", Length = 200)]
    public virtual string Name { get; set; }
    /// <summary>
    /// 编码 
    ///</summary>
    [SugarColumn(ColumnName = "Code", ColumnDescription = "编码", Length = 200)]
    public string Code { get; set; }
    /// <summary>
    /// 分类 
    ///</summary>
    [SugarColumn(ColumnName = "Category", ColumnDescription = "分类", Length = 200)]
    public virtual string Category { get; set; }
    /// <summary>
    /// 排序码 
    ///</summary>
    [SugarColumn(ColumnName = "SortCode", ColumnDescription = "排序码", IsNullable = true)]
    public int? SortCode { get; set; }

}
