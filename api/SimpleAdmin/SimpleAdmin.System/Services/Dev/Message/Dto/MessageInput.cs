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
public class MessageSendInput : SysMessage, IValidatableObject
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
        if (Category != CateGoryConst.MESSAGE_INFORM && Category != CateGoryConst.MESSAGE_NOTICE)
        {
            yield return new ValidationResult("分类错误", new[] { nameof(Category) });
        }
    }
}
