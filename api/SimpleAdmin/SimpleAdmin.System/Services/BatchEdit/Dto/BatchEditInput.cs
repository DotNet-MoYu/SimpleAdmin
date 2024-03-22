// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.System;

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
    public string EntityName { get; set; }

    /// <summary>
    /// 表名
    /// </summary>
    public string TableName { get; set; }
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
    public string TableDescription { get; set; }
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
        if (Status == CommonStatusConst.ENABLE)
        {
            //如果是api请求并且必填参数有空的
            if (DataType.Contains("api") && (string.IsNullOrEmpty(RequestUrl) || string.IsNullOrEmpty(RequestType)
                || string.IsNullOrEmpty(RequestLabel) || string.IsNullOrEmpty(RequestValue)))
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
