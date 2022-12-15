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



    public string Account { get; set; }


    public string Name { get; set; }

}

