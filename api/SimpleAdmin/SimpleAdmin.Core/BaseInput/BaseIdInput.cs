namespace SimpleAdmin.Core;

/// <summary>
/// 主键Id输入参数
/// </summary>
public class BaseIdInput
{
    /// <summary>
    /// 主键Id
    /// </summary>
    [IdNotNull(ErrorMessage = "Id不能为空")]
    [DataValidation(ValidationTypes.Numeric)]
    public virtual long Id { get; set; }
}

public class BaseIdListInput
{
    /// <summary>
    /// 主键Id列表
    /// </summary>
    [IdsNotNull(ErrorMessage = "IdList不能为空")]
    public List<long> Ids { get; set; } = new List<long>();
}

/// <summary>
/// Id列表输入
/// </summary>
public class IdListInput
{
    [Required(ErrorMessage = "IdList不能为空")]
    public List<long>? IdList { get; set; }
}