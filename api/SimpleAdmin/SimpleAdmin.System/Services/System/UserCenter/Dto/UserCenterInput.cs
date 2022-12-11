namespace SimpleAdmin.System;

/// <summary>
/// 编辑个人信息参数
/// </summary>
public class UserUpdateInfoInput : SysUser
{
    /// <summary>
    /// Id
    /// </summary>
    [MinValue(1, ErrorMessage = "Id不能为空")]
    public override long Id { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    [Required(ErrorMessage = "Name不能为空")]
    public override string Name { get; set; }
}
