namespace SimpleAdmin.Core;

/// <summary>
/// 验证Id不能为 空
/// </summary>
public class IdNotNull : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value.ToLong() > 0 || !string.IsNullOrEmpty(value.ToString()))
            return true;
        else return false;
    }
}
