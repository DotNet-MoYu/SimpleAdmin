namespace SimpleAdmin.Plugin.SqlSugar;

/// <summary>
/// 字典
///</summary>
[SugarTable("dev_dict", TableDescription = "字典表")]
[Tenant(SqlsugarConst.DB_Default)]
public class DevDict : BaseEntity
{
    /// <summary>
    /// 父id 
    ///</summary>
    [SugarColumn(ColumnName = "ParentId", ColumnDescription = "父id")]
    public virtual long ParentId { get; set; }
    /// <summary>
    /// 字典文字 
    ///</summary>
    [SugarColumn(ColumnName = "DictLabel", ColumnDescription = "字典文字", Length = 200)]
    public virtual string DictLabel { get; set; }
    /// <summary>
    /// 字典值 
    ///</summary>
    [SugarColumn(ColumnName = "DictValue", ColumnDescription = "字典值", Length = 200)]
    public virtual string DictValue { get; set; }
    /// <summary>
    /// 分类 
    ///</summary>
    [SugarColumn(ColumnName = "Category", ColumnDescription = "分类", Length = 200)]
    public string Category { get; set; }

    /// <summary>
    /// 排序码 
    ///</summary>
    [SugarColumn(ColumnName = "SortCode", ColumnDescription = "排序码")]
    public int SortCode { get; set; }


    /// <summary>
    /// 子节点
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<DevDict> Children { get; set; }


}
