// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.Web.Core;

/// <summary>
/// 用户个人信息控制器
/// </summary>
[ApiDescriptionSettings(Tag = "用户个人信息控制器")]
[Route("userCenter")]
public class UserCenterController : BaseController
{
    private readonly IUserCenterService _userCenterService;

    public UserCenterController(IUserCenterService userCenterService)
    {
        _userCenterService = userCenterService;
    }



    #region Get

    /// <summary>
    /// 获取个人菜单
    /// </summary>
    /// <returns></returns>
    [HttpGet("loginMenu")]
    public async Task<dynamic> LoginMenu([FromQuery] BaseIdInput input)
    {
        return await _userCenterService.GetLoginMenu(input);
    }

    /// <summary>
    /// 获取个人工作台
    /// </summary>
    /// <returns></returns>
    [HttpGet("loginWorkbench")]
    public async Task<dynamic> LoginWorkbench()
    {
        return await _userCenterService.GetLoginWorkbench();
    }


    /// <summary>
    /// 获取组织架构
    /// </summary>
    /// <returns></returns>
    [HttpGet("loginOrgTree")]
    public async Task<dynamic> LoginOrgTree()
    {
        return await _userCenterService.LoginOrgTree();
    }



    /// <summary>
    /// 获取登录用户的站内信分页
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("loginUnreadMessagePage")]
    public async Task<dynamic> LoginUnreadMessagePage([FromQuery] MessagePageInput input)
    {
        return await _userCenterService.LoginMessagePage(input);
    }

    /// <summary>
    /// 读取登录用户站内信详情
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("loginUnreadMessageDetail")]
    public async Task<dynamic> LoginUnreadMessageDetail([FromQuery] BaseIdInput input)
    {
        return await _userCenterService.LoginMessageDetail(input);
    }

    /// <summary>
    /// 未读消息数
    /// </summary>
    /// <returns></returns>
    [HttpGet("UnReadCount")]
    public async Task<dynamic> UnReadCount()
    {
        return await _userCenterService.UnReadCount();
    }


    /// <summary>
    /// 删除我的消息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("deleteMessage")]
    [DisplayName("删除个人消息")]
    public async Task DeleteMessage([FromBody] BaseIdInput input)
    {
        await _userCenterService.DeleteMyMessage(input);
    }

    /// <summary>
    /// 修改密码
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("updatePassword")]
    [DisplayName("修改密码")]
    public async Task UpdatePassword([FromBody] UpdatePasswordInput input)
    {
        await _userCenterService.UpdatePassword(input);
    }

    /// <summary>
    /// 修改头像
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("updateAvatar")]
    [DisplayName("修改头像")]
    public async Task<dynamic> UpdateAvatar([FromForm] BaseFileInput input)
    {
        return await _userCenterService.UpdateAvatar(input);
    }


    /// <summary>
    /// 修改默认模块
    /// </summary>
    /// <param name="input"></param>
    [HttpPost("setDefaultModule")]
    public async Task SetDefaultModule([FromBody] SetDefaultModuleInput input)
    {
        await _userCenterService.SetDefaultModule(input);
    }


    /// <summary>
    /// 快捷方式菜单树
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("shortcutTree")]
    public async Task<dynamic> ShortcutTree()
    {
        return await _userCenterService.ShortcutTree();
    }

    #endregion

    #region Post

    /// <summary>
    /// 编辑个人信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("updateUserInfo")]
    [DisplayName("编辑个人信息")]
    public async Task UpdateUserInfo([FromBody] UpdateInfoInput input)
    {
        await _userCenterService.UpdateUserInfo(input);
    }

    /// <summary>
    /// 更新签名
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("updateSignature")]
    [DisplayName("更新签名")]
    public async Task UpdateSignature([FromBody] UpdateSignatureInput input)
    {
        await _userCenterService.UpdateSignature(input);
    }



    /// <summary>
    /// 编辑工作台
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("updateUserWorkbench")]
    [DisplayName("编辑工作台")]
    public async Task UpdateUserWorkbench([FromBody] UpdateWorkbenchInput input)
    {
        await _userCenterService.UpdateWorkbench(input);
    }

    #endregion
}
