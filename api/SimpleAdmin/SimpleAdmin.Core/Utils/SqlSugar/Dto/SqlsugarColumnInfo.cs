namespace SimpleAdmin.Core.Utils;


/// <summary>
/// Sqlsugar字段信息
/// </summary>
public class SqlsugarColumnInfo
{
    /// <summary>
    /// 字段名称
    /// </summary>
    public string ColumnName { get; set; }


    /// <summary>
    /// 是否主键
    /// </summary>
    public bool IsPrimarykey { get; set; }

    /// <summary>
    /// 字段类型
    /// </summary>
    public string TypeName { get; set; }

    /// <summary>
    /// 字段注释
    /// </summary>
    public string ColumnDescription { get; set; }
}
