namespace SimpleAdmin.Plugin.SqlSugar;

/// <summary>
/// 批量修改
///</summary>
[SugarTable("batch_edit", TableDescription = "批量修改")]
[Tenant(SqlsugarConst.DB_Default)]
[CodeGen]
public class BatchEdit : PrimaryKeyEntity
{
    /// <summary>
    /// 唯一编码
    ///</summary>
    [SugarColumn(ColumnName = "Code", ColumnDescription = "唯一编码", Length = 100, IsNullable = false)]
    public string Code { get; set; }

    /// <summary>
    /// 所属库
    ///</summary>
    [SugarColumn(ColumnName = "ConfigId", ColumnDescription = "所属库", Length = 100, IsNullable = false)]
    public string ConfigId { get; set; }

    /// <summary>
    /// 实体名
    ///</summary>
    [SugarColumn(ColumnName = "EntityName", ColumnDescription = "实体名", Length = 100, IsNullable = false)]
    public string EntityName { get; set; }

    /// <summary>
    /// 表名
    ///</summary>
    [SugarColumn(ColumnName = "TableName", ColumnDescription = "表名", Length = 100, IsNullable = false)]
    public string TableName { get; set; }

    /// <summary>
    /// 表描述
    ///</summary>
    [SugarColumn(ColumnName = "TableDescription", ColumnDescription = "表描述", Length = 100, IsNullable = false)]
    public string TableDescription { get; set; }
}