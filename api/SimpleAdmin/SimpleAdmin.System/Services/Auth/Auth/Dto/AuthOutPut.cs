namespace SimpleAdmin.System;

public class AuthOutPut
{

}

/// <summary>
/// 验证码返回
/// </summary>
public class PicValidCodeOutPut
{
    /// <summary>
    /// 验证码图片，Base64
    /// </summary>
    public string ValidCodeBase64 { get; set; }

    /// <summary>
    /// 验证码请求号
    /// </summary>
    public string ValidCodeReqNo { get; set; }

}

/// <summary>
/// 登录返回参数
/// </summary>
public class LoginOutPut
{
    /// <summary>
    /// 令牌Token
    /// </summary>
    public string Token { get; set; }


    /// <summary>
    /// 账号
    /// </summary>
    public string Account { get; set; }


    /// <summary>
    /// 姓名
    /// </summary>
    public string Name { get; set; }

}

/// <summary>
/// 登录用互信息
/// </summary>
public class LoginUserOutput
{
    /// <summary>
    /// 头像 
    ///</summary>
    public string Avatar { get; set; }

    /// <summary>
    /// 签名 
    ///</summary>
    public string Signature { get; set; }

    /// <summary>
    /// 账号 
    ///</summary>
    public string Account { get; set; }


    /// <summary>
    /// 姓名 
    ///</summary>
    public string Name { get; set; }

    /// <summary>
    /// 昵称 
    ///</summary>
    public string Nickname { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// 生日
    /// </summary>
    public string Birthday { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>

    public string Email { get; set; }

    /// <summary>
    /// 性别 
    ///</summary>
    public string Gender { get; set; }

    /// <summary>
    /// 家庭住址 
    ///</summary>
    public string HomeAddress { get; set; }

    /// <summary>
    /// 机构信息
    /// </summary>
    public string OrgName { get; set; }

    /// <summary>
    /// 机构信息全称
    /// </summary>
    public string OrgNames { get; set; }

    /// <summary>
    /// 职位信息
    /// </summary>
    public string PositionName { get; set; }

    /// <summary>
    /// 角色码集合
    /// </summary>
    public List<string> RoleCodeList { get; set; }

    /// <summary>
    /// 按钮码集合
    /// </summary>
    public List<string> ButtonCodeList { get; set; }

}