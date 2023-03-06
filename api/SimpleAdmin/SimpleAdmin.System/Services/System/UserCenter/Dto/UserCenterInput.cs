namespace SimpleAdmin.System;

/// <summary>
/// 编辑个人信息参数
/// </summary>
public class UpdateInfoInput : SysUser
{
    /// <summary>
    /// Id
    /// </summary>
    [IdNotNull(ErrorMessage = "Id不能为空")]
    public override long Id { get; set; }

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
{    /// <summary>
     /// 工作台数据
     /// </summary>
    [Required(ErrorMessage = "WorkbenchData不能为空")]
    public string WorkbenchData { get; set; }
}