namespace SimpleAdmin.Plugin.SqlSugar;

/// <summary>
/// Sqlsugar表信息
/// </summary>
public class SqlSugarTableInfo
{
    /// <summary>
    /// 所属库
    ///</summary>
    public virtual string ConfigId { get; set; }

    /// <summary>
    /// 表名称
    /// </summary>
    public string TableName { get; set; }

    /// <summary>
    /// 实体名
    /// </summary>
    public string EntityName { get; set; }

    /// <summary>
    /// 表注释
    /// </summary>
    public string TableDescription { get; set; }

    /// <summary>
    /// 表字段
    /// </summary>
    public List<SqlsugarColumnInfo> Columns { get; set; }
}