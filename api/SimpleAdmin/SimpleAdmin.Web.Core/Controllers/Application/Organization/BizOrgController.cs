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
/// 机构管理控制器
/// </summary>
[ApiDescriptionSettings("Application", Tag = "机构管理")]
[Route("biz/organization/org")]
[RolePermission]
public class BizOrgController : IDynamicApiController
{
    private readonly IOrgService _orgService;
    private readonly IUserService _userService;

    public BizOrgController(IOrgService orgService, IUserService userService)
    {
        _orgService = orgService;
        _userService = userService;
    }

    #region Get请求

    /// <summary>
    /// 获取机构树
    /// </summary>
    /// <returns></returns>
    [HttpGet("tree")]
    [DisplayName("机构树查询")]
    public async Task<dynamic> Tree()
    {
        return await _orgService.Tree();
    }

    /// <summary>
    /// 机构分页查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("page")]
    [DisplayName("机构分页查询")]
    public async Task<dynamic> Page([FromQuery] SysOrgPageInput input)
    {
        return await _orgService.Page(input);
    }

    /// <summary>
    /// 获取机构树选择器
    /// </summary>
    /// <returns></returns>
    [HttpGet("orgTreeSelector")]
    [DisplayName("机构树选择器")]
    public async Task<dynamic> OrgTreeSelector()
    {
        return await _orgService.Tree();
    }

    /// <summary>
    /// 获取人员选择器
    /// </summary>
    /// <returns></returns>
    [HttpGet("userSelector")]
    [DisplayName("人员选择器")]
    public async Task<dynamic> UserSelector([FromQuery] UserSelectorInput input)
    {
        return await _userService.Selector(input);
    }

    /// <summary>
    /// 机构详情
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("detail")]
    [DisplayName("机构详情")]
    public async Task<dynamic> Detail([FromQuery] BaseIdInput input)
    {
        return await _orgService.Detail(input);
    }

    #endregion

    #region Post请求

    /// <summary>
    /// 添加机构
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [DisplayName("添加机构")]
    public async Task Add([FromBody] SysOrgAddInput input)
    {
        await _orgService.Add(input);
    }

    /// <summary>
    /// 修改机构
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("edit")]
    [DisplayName("修改机构")]
    public async Task Edit([FromBody] SysOrgEditInput input)
    {
        await _orgService.Edit(input);
    }

    /// <summary>
    /// 复制组织
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("copy")]
    [DisplayName("复制组织")]
    public async Task Copy(SysOrgCopyInput input)
    {
        await _orgService.Copy(input);
    }

    /// <summary>
    /// 删除机构
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("delete")]
    [DisplayName("删除机构")]
    public async Task Delete([FromBody] BaseIdListInput input)
    {
        await _orgService.Delete(input);
    }

    #endregion
}
