namespace SimpleAdmin.Plugin.Batch;

/// <summary>
/// 批量分页查询参数
/// </summary>
public class BatchEditPageInput : BasePageInput
{
    /// <summary>
    /// 唯一编码
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 所属库
    /// </summary>
    public string ConfigId { get; set; }

    /// <summary>
    /// 实体名
    /// </summary>
    public string Entityname { get; set; }

    /// <summary>
    /// 表名
    /// </summary>
    public string Tablename { get; set; }
}

/// <summary>
/// 添加批量参数
/// </summary>
public class BatchEditAddInput
{
    /// <summary>
    /// 唯一编码
    /// </summary>
    [Required(ErrorMessage = "Code必填")]
    public string Code { get; set; }

    /// <summary>
    /// 所属库
    /// </summary>
    [Required(ErrorMessage = "ConfigId必填")]
    public string ConfigId { get; set; }

    /// <summary>
    /// 实体名
    /// </summary>
    [Required(ErrorMessage = "EntityName必填")]
    public string EntityName { get; set; }

    /// <summary>
    /// 表名
    /// </summary>
    [Required(ErrorMessage = "TableName必填")]
    public string TableName { get; set; }

    /// <summary>
    /// 表描述
    /// </summary>
    public string Tabledescription { get; set; }
}

/// <summary>
/// 修改批量参数
/// </summary>
public class BatchEditConfigInput : BatchEditConfig, IValidatableObject
{
    /// <summary>
    /// Id
    /// </summary>
    [IdNotNull(ErrorMessage = "Id不能为空")]
    public override long Id { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Status == DevDictConst.COMMON_STATUS_ENABLE)
        {
            //如果是api请求并且必填参数有空的
            if (DataType.Contains("api") && (string.IsNullOrEmpty(RequestUrl) || string.IsNullOrEmpty(RequestType) || string.IsNullOrEmpty(RequestLabel) || string.IsNullOrEmpty(RequestValue)))
            {
                yield return new ValidationResult($"字段{ColumnName}接口信息必填", new[] { nameof(DataType) });
            }
            //如果是字典数据并且字典值为空
            if (DataType.Contains("dict") && string.IsNullOrEmpty(DictTypeCode))
            {
                yield return new ValidationResult($"字段{ColumnName}字典值必填", new[] { nameof(DictTypeCode) });
            }
        }
    }
}

/// <summary>
/// 批量修改输入
/// </summary>
public class BatchEditInput
{
    /// <summary>
    /// 批量编辑Code
    /// </summary>
    [Required(ErrorMessage = "Code不能为空")]
    public string Code { get; set; }

    /// <summary>
    /// Id列表
    /// </summary>
    [Required(ErrorMessage = "Ids不能为空")]
    public List<long>? Ids { get; set; }

    /// <summary>
    /// 字段列表
    /// </summary>
    [Required(ErrorMessage = "Columns不能为空")]
    public List<BatchEditColumn>? Columns { get; set; }
}

/// <summary>
/// 批量修改DTO
/// </summary>
public class BatchEditColumn
{
    [Required(ErrorMessage = "字段名必填")]
    public string TableColumn { get; set; }

    [Required(ErrorMessage = "字段值必填")]
    public object ColumnValue { get; set; }
}