// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.Core;

/// <summary>
/// 全局分页查询输入参数
/// </summary>
public class BasePageInput : IValidatableObject
{
    /// <summary>
    /// 当前页码
    /// </summary>
    [DataValidation(ValidationTypes.Numeric)]
    public virtual int PageNum { get; set; } = 1;

    /// <summary>
    /// 每页条数
    /// </summary>
    [Range(1, 100, ErrorMessage = "页码容量超过最大限制")]
    [DataValidation(ValidationTypes.Numeric)]
    public virtual int PageSize { get; set; } = 10;

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
            var fields = SortField.Split(" ");
            if (fields.Length > 1)
            {
                yield return new ValidationResult("排序字段错误", new[]
                {
                    nameof(SortField)
                });
            }
        }
    }
}
