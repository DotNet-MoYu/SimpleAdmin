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
public class SetDeafultModuleInput
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