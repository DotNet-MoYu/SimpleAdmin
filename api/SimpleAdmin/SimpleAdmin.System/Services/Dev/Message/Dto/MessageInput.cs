namespace SimpleAdmin.System;

public class MessagePageInput : BasePageInput
{
    /// <summary>
    /// 站内信分类
    /// </summary>
    public string Category { get; set; }
}

/// <summary>
/// 发送参数
/// </summary>
public class MessageSendInput : DevMessage, IValidatableObject
{
    /// <summary>
    /// 主题
    /// </summary>
    [Required(ErrorMessage = "Subject不能为空")]
    public override string Subject { get; set; }

    /// <summary>
    /// 分类
    /// </summary>
    [Required(ErrorMessage = "Category不能为空")]
    public override string Category { get; set; }

    /// <summary>
    /// 接收人Id
    /// </summary>
    [Required(ErrorMessage = "ReceiverIdList不能为空")]
    public List<long> ReceiverIdList { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Category != CateGoryConst.Message_INFORM && Category != CateGoryConst.Message_NOTICE)
        {
            yield return new ValidationResult("分类错误", new[] { nameof(Category) });
        }
    }
}