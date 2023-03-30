namespace SimpleAdmin.Core;

/// <summary>
/// 全局分页查询输入参数
/// </summary>
public class BasePageInput : IValidatableObject
{
    /// <summary>
    /// 当前页码
    /// </summary>
    [System.ComponentModel.DataAnnotations.DataValidation(ValidationTypes.Numeric)]
    public virtual int Current { get; set; } = 1;

    /// <summary>
    /// 每页条数
    /// </summary>
    [Range(1, 100, ErrorMessage = "页码容量超过最大限制")]
    [System.ComponentModel.DataAnnotations.DataValidation(ValidationTypes.Numeric)]
    public virtual int Size { get; set; } = 10;

    /// <summary>
    /// 排序字段
    /// </summary>
    public virtual string SortField { get; set; }

    /// <summary>
    /// 排序方式，升序：ascend；降序：descend"
    /// </summary>
    public virtual string SortOrder { get; set; } = "desc";


    /// <summary>
    /// 关键字
    /// </summary>
    public virtual string SearchKey { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        //配合小诺排序参数
        if (SortOrder == "descend")
        {
            SortOrder = "desc";
        }
        else if (SortOrder == "ascend")
        {
            SortOrder = "asc";
        }
        if (!string.IsNullOrEmpty(SortField))
        {
            //分割排序字段
            var fields = SortField.Split();
            if (fields.Length > 0)
            {
                yield return new ValidationResult($"排序字段错误", new[] { nameof(SortField) });
            }
        }
        yield break;
    }
}
