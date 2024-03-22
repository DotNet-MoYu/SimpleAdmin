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
/// 编辑个人信息参数
/// </summary>
public class UpdateInfoInput : SysUser
{
    /// <summary>
    /// 姓名
    /// </summary>
    [Required(ErrorMessage = "Name不能为空")]
    public override string Name { get; set; }
}

public class UpdateSignatureInput
{
    /// <summary>
    /// 签名图片
    /// </summary>
    [Required(ErrorMessage = "Signature签名图片不能为空")]
    public string Signature { get; set; }
}

/// <summary>
/// 更新个人工作台
/// </summary>
public class UpdateWorkbenchInput
{
    /// <summary>
    /// 工作台数据
    /// </summary>
    [Required(ErrorMessage = "WorkbenchData不能为空")]
    public string WorkbenchData { get; set; }
}

/// <summary>
/// 更新个人密码
/// </summary>
public class UpdatePasswordInput
{
    /// <summary>
    /// 密码
    /// </summary>
    [Required(ErrorMessage = "Password不能为空")]
    public string Password { get; set; }

    /// <summary>
    /// 新密码
    /// </summary>
    [Required(ErrorMessage = "NewPassword不能为空")]
    public string NewPassword { get; set; }
}

/// <summary>
/// 设置默认模块输入
/// </summary>
public class SetDefaultModuleInput
{
    /// <summary>
    /// 模块Id
    /// </summary>
    [Required(ErrorMessage = "Id不能为空")]
    public long Id { get; set; }

    /// <summary>
    /// 是否默认
    /// </summary>
    [Required(ErrorMessage = "IfDefault不能为空")]
    public bool IfDefault { get; set; }
}
