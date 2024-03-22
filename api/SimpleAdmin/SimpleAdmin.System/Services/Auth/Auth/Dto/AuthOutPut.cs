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

    /// <summary>
    /// 默认模块
    /// </summary>
    public long? DefaultModule { get; set; }

    /// <summary>
    /// 模块列表
    /// </summary>
    public List<SysResource> ModuleList { get; set; }
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

    /// <summary>
    /// 机构及以下机构ID集合
    /// </summary>
    public List<SysResource> ModuleList { get; set; } = new List<SysResource>();

    /// <summary>
    /// 默认模块
    ///</summary>
    public long? DefaultModule { get; set; }
}
