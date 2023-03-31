namespace SimpleAdmin.Plugin.SqlSugar;

/// <summary>
/// 批量修改配置表
///</summary>
[SugarTable("batch_edit_config", TableDescription = "批量修改配置表")]
[Tenant(SqlsugarConst.DB_Default)]
public class BatchEditConfig : PrimaryKeyEntity
{
    /// <summary>
    /// 批量配置Id 
    ///</summary>
    [SugarColumn(ColumnName = "UId", ColumnDescription = "批量配置Id", IsNullable = false)]
    public long UId { get; set; }
    /// <summary>
    /// 字段名 
    ///</summary>
    [SugarColumn(ColumnName = "ColumnName", ColumnDescription = "字段名", Length = 100, IsNullable = false)]
    public string ColumnName { get; set; }
    /// <summary>
    /// 字段描述 
    ///</summary>
    [SugarColumn(ColumnName = "ColumnComment", ColumnDescription = "字段描述", Length = 100, IsNullable = false)]
    public string ColumnComment { get; set; }
    /// <summary>
    /// 作用类型 
    ///</summary>
    [SugarColumn(ColumnName = "DataType", ColumnDescription = "作用类型", Length = 100, IsNullable = false)]
    public string DataType { get; set; }
    /// <summary>
    /// 字典值 
    ///</summary>
    [SugarColumn(ColumnName = "DictTypeCode", ColumnDescription = "字典值", Length = 100, IsNullable = true)]
    public string DictTypeCode { get; set; }
    /// <summary>
    /// 数据库类型 
    ///</summary>
    [SugarColumn(ColumnName = "NetType", ColumnDescription = "数据库类型", Length = 100, IsNullable = true)]
    public string NetType { get; set; }

    /// <summary>
    /// 接口名称
    /// </summary>
    [SugarColumn(ColumnName = "RequestUrl", ColumnDescription = "接口名称", Length = 100, IsNullable = true)]
    public string RequestUrl { get; set; }

    /// <summary>
    /// 接口类型
    /// </summary>
    [SugarColumn(ColumnName = "RequestType", ColumnDescription = "接口类型", Length = 100, IsNullable = true)]
    public string RequestType { get; set; }


    /// <summary>
    /// 接口结果标签
    /// </summary>
    [SugarColumn(ColumnName = "RequestLabel", ColumnDescription = "接口结果标签", Length = 100, IsNullable = true)]
    public string RequestLabel { get; set; }

    /// <summary>
    /// 接口结果值
    /// </summary>
    [SugarColumn(ColumnName = "RequestValue", ColumnDescription = "接口结果值", Length = 100, IsNullable = true)]
    public string RequestValue { get; set; }

    /// <summary>
    /// 启用状态
    /// </summary>
    [SugarColumn(ColumnName = "Status", ColumnDescription = "状态", Length = 100, IsNullable = false)]
    public string Status { get; set; }
}

