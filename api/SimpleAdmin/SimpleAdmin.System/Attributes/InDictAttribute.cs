namespace SimpleAdmin.System;

/// <summary>
/// 验证值是否在字典中,大数据慎用
/// </summary>
public class InDictAttribute : ValidationAttribute
{
    public InDictAttribute(string dictValue)
    {
        DictValue = dictValue;
    }

    /// <summary>
    /// 字典值
    /// </summary>
    public string DictValue { get; set; }

    /// <summary>
    /// 获取错误信息
    /// </summary>
    /// <param name="value">值</param>
    /// <returns></returns>
    public string GetErrorMessage(string value)
    {
        if (string.IsNullOrEmpty(ErrorMessage))
        {
            switch (DictValue)
            {
                case DevDictConst.GENDER:
                    return "性别只能是男和女";

                case DevDictConst.NATION:
                    return "不存在的民族";

                default:
                    return $"字典中不存在{value}";
            }
        }
        else return ErrorMessage;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value != null)
        {
            var dictSerivce = App.GetService<IDictService>();
            var values = dictSerivce.GetValuesByDictValue(DictValue).Result;
            if (!values.Contains(value))
            {
                return new ValidationResult(GetErrorMessage(value.ToString()), new string[] { validationContext.MemberName });
            }
        }
        return ValidationResult.Success;
    }
}