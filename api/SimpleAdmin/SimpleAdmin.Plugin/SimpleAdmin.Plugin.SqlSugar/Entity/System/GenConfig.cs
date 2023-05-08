namespace SimpleAdmin.Plugin.SqlSugar;

/// <summary>
/// 代码生成配置
///</summary>
[SugarTable("gen_config", TableDescription = "代码生成配置")]
[Tenant(SqlsugarConst.DB_Default)]
public class GenConfig : BaseEntity
{
    /// <summary>
    /// 基础ID
    ///</summary>
    [SugarColumn(ColumnName = "BasicId", ColumnDescription = "基础配置ID")]
    public long BasicId { get; set; }

    /// <summary>
    /// 是否主键
    ///</summary>
    [SugarColumn(ColumnName = "IsPrimarykey", ColumnDescription = "是否主键", Length = 200)]
    public string IsPrimarykey { get; set; }

    /// <summary>
    /// 字段
    ///</summary>
    [SugarColumn(ColumnName = "FieldName", ColumnDescription = "字段", Length = 200)]
    public string FieldName { get; set; }

    /// <summary>
    /// 名称
    ///</summary>
    [SugarColumn(ColumnName = "FieldRemark", ColumnDescription = "名称", Length = 200)]
    public string FieldRemark { get; set; }

    /// <summary>
    /// 类型
    ///</summary>
    [SugarColumn(ColumnName = "FieldType", ColumnDescription = "类型", Length = 200)]
    public string FieldType { get; set; }

    /// <summary>
    /// 实体类型
    ///</summary>
    [SugarColumn(ColumnName = "FieldNetType", ColumnDescription = "实体类型", Length = 200)]
    public string FieldNetType { get; set; }

    /// <summary>
    /// 作用类型
    ///</summary>
    [SugarColumn(ColumnName = "EffectType", ColumnDescription = "作用类型", Length = 200)]
    public string EffectType { get; set; }

    /// <summary>
    /// 字典
    ///</summary>
    [SugarColumn(ColumnName = "DictTypeCode", ColumnDescription = "字典", Length = 200, IsNullable = true)]
    public string DictTypeCode { get; set; }

    /// <summary>
    /// 列表显示
    ///</summary>
    [SugarColumn(ColumnName = "WhetherTable", ColumnDescription = "列表显示", Length = 200)]
    public string WhetherTable { get; set; }

    /// <summary>
    /// 列省略
    ///</summary>
    [SugarColumn(ColumnName = "WhetherRetract", ColumnDescription = "列省略", Length = 200)]
    public string WhetherRetract { get; set; }

    /// <summary>
    /// 是否增改
    ///</summary>
    [SugarColumn(ColumnName = "WhetherAddUpdate", ColumnDescription = "是否增改", Length = 200)]
    public string WhetherAddUpdate { get; set; }

    /// <summary>
    /// 是否导入导出
    ///</summary>
    [SugarColumn(ColumnName = "WhetherImportExport", ColumnDescription = "是否导入导出", Length = 200)]
    public string WhetherImportExport { get; set; }

    /// <summary>
    /// 必填
    ///</summary>
    [SugarColumn(ColumnName = "WhetherRequired", ColumnDescription = "必填", Length = 200)]
    public string WhetherRequired { get; set; }

    /// <summary>
    /// 查询
    ///</summary>
    [SugarColumn(ColumnName = "QueryWhether", ColumnDescription = "查询", Length = 200)]
    public string QueryWhether { get; set; }

    /// <summary>
    /// 查询方式
    ///</summary>
    [SugarColumn(ColumnName = "QueryType", ColumnDescription = "查询方式", Length = 200, IsNullable = true)]
    public string QueryType { get; set; }

    /// <summary>
    /// 排序
    ///</summary>
    [SugarColumn(ColumnName = "SortCode", ColumnDescription = "排序")]
    public int SortCode { get; set; }

    /// <summary>
    /// 字段名首字母小写
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public string FieldNameFirstLower { get; set; }

    /// <summary>
    /// 字段名首字母大写
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public string FieldNameFirstUpper { get; set; }
}