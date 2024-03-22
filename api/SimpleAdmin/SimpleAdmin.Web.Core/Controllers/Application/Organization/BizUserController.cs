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
/// 人员管理控制器
/// </summary>
[ApiDescriptionSettings("Application", Tag = "人员管理")]
[Route("biz/organization/user")]
[RolePermission]
public class BizUserController : IDynamicApiController
{
    private readonly IUserService _userService;
    private readonly IOrgService _orgService;
    private readonly IPositionService _positionService;

    public BizUserController(IUserService userService, IOrgService orgService, IPositionService positionService)
    {
        _userService = userService;
        _orgService = orgService;
        _positionService = positionService;
    }

    #region Get请求

    /// <summary>
    /// 导入预览
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("preview")]
    [DisableRequestSizeLimit]
    [SuppressMonitor]
    [DisplayName("导入预览")]
    public async Task<dynamic> Preview([FromForm] ImportPreviewInput input)
    {
        return await _userService.Preview(input);
    }

    /// <summary>
    /// 模板下载
    /// </summary>
    /// <returns></returns>
    [HttpGet(template: "template")]
    [SuppressMonitor]
    public async Task<dynamic> Template()
    {
        return await _userService.Template();
    }


    /// <summary>
    /// 人员分页查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("page")]
    [DisplayName("人员分页查询")]
    public async Task<dynamic> Page([FromQuery] UserPageInput input)
    {
        return await _userService.Page(input);
    }

    /// <summary>
    /// 获取人员选择器
    /// </summary>
    /// <returns></returns>
    [HttpGet("selector")]
    [DisplayName("人员选择器")]
    public async Task<dynamic> Selector([FromQuery] UserSelectorInput input)
    {
        return await _userService.Selector(input);
    }

    /// <summary>
    /// 获取人员拥有角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("ownRole")]
    [DisplayName("获取人员拥有角色")]
    public async Task<dynamic> OwnRole([FromQuery] BaseIdInput input)
    {
        return await _userService.OwnRole(input);
    }

    /// <summary>
    /// 人员详情
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("detail")]
    [DisplayName("人员详情")]
    public async Task<dynamic> Detail([FromQuery] BaseIdInput input)
    {
        return await _userService.Detail(input);
    }

    #endregion Get请求

    #region Post请求

    /// <summary>
    /// 添加人员
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [DisplayName("添加人员")]
    public async Task Add([FromBody] UserAddInput input)
    {
        await _userService.Add(input);
    }

    /// <summary>
    /// 修改人员
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("edit")]
    [DisplayName("修改人员")]
    public async Task Edit([FromBody] UserEditInput input)
    {
        await _userService.Edit(input);
    }

    /// <summary>
    /// 批量修改人员
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("edits")]
    [DisplayName("批量修改人员")]
    public async Task Edits([FromBody] BatchEditInput input)
    {
        await _userService.Edits(input);
    }

    /// <summary>
    /// 删除人员
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("delete")]
    [DisplayName("删除人员")]
    public async Task Delete([FromBody] BaseIdListInput input)
    {
        await _userService.Delete(input);
    }

    /// <summary>
    /// 禁用人员
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("disableUser")]
    [DisplayName("禁用人员")]
    public async Task DisableUser([FromBody] BaseIdInput input)
    {
        await _userService.DisableUser(input);
    }

    /// <summary>
    /// 启用人员
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("enableUser")]
    [DisplayName("启用人员")]
    public async Task EnableUser([FromBody] BaseIdInput input)
    {
        await _userService.EnableUser(input);
    }

    /// <summary>
    /// 重置密码
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("resetPassword")]
    [DisplayName("重置密码")]
    public async Task ResetPassword([FromBody] BaseIdInput input)
    {
        await _userService.ResetPassword(input);
    }

    /// <summary>
    /// 给人员授权角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("grantRole")]
    [DisplayName("授权角色")]
    public async Task GrantRole([FromBody] UserGrantRoleInput input)
    {
        await _userService.GrantRole(input);
    }

    /// <summary>
    /// 人员导入
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("import")]
    [DisplayName("人员导入")]
    public async Task<dynamic> Import([SuppressMonitor][FromBody] ImportResultInput<BizUserImportInput> input)
    {
        return await _userService.Import(input);
    }

    /// <summary>
    /// 人员导出
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("export")]
    [DisplayName("人员导出")]
    public async Task<dynamic> Export([FromBody] UserPageInput input)
    {
        return await _userService.Export(input);
    }

    #endregion Post请求
}
