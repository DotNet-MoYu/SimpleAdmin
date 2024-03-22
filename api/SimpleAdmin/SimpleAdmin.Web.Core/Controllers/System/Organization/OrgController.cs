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
/// 组织管理控制器
/// </summary>
[ApiDescriptionSettings(Tag = "组织管理")]
[Route("sys/organization/[controller]")]
[SuperAdmin]
public class OrgController : BaseController
{
    private readonly ISysOrgService _sysOrgService;
    private readonly ISysUserService _sysUserService;

    public OrgController(ISysOrgService sysOrgService, ISysUserService sysUserService)
    {
        _sysOrgService = sysOrgService;
        _sysUserService = sysUserService;
    }

    #region Get

    /// <summary>
    /// 获取组织树
    /// </summary>
    /// <returns></returns>
    [HttpGet("tree")]
    public async Task<dynamic> Tree()
    {
        return await _sysOrgService.Tree();
    }

    /// <summary>
    /// 获取组织树选择器
    /// </summary>
    /// <returns></returns>
    [HttpGet("orgTreeSelector")]
    public async Task<dynamic> OrgTreeSelector()
    {
        return await _sysOrgService.Tree();
    }

    /// <summary>
    /// 组织分页查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("page")]
    public async Task<dynamic> Page([FromQuery] SysOrgPageInput input)
    {
        return await _sysOrgService.Page(input);
    }

    /// <summary>
    /// 获取用户选择器
    /// </summary>
    /// <returns></returns>
    [HttpGet("selector")]
    public async Task<dynamic> UserSelector([FromQuery] UserSelectorInput input)
    {
        return await _sysUserService.Selector(input);
    }

    /// <summary>
    /// 获取组织详情
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("detail")]
    public async Task<dynamic> Detail([FromQuery] BaseIdInput input)
    {
        return await _sysOrgService.Detail(input);
    }

    #endregion Get

    #region Post

    /// <summary>
    /// 复制组织
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("copy")]
    [DisplayName("复制组织")]
    public async Task Copy([FromBody] SysOrgCopyInput input)
    {
        await _sysOrgService.Copy(input);
    }

    /// <summary>
    /// 添加组织
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [DisplayName("添加组织")]
    public async Task Add([FromBody] SysOrgAddInput input)
    {
        await _sysOrgService.Add(input);
    }

    /// <summary>
    /// 修改组织
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("edit")]
    [DisplayName("修改组织")]
    public async Task Edit([FromBody] SysOrgEditInput input)
    {
        await _sysOrgService.Edit(input);
    }

    /// <summary>
    /// 删除组织
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("delete")]
    [DisplayName("删除组织")]
    public async Task Delete([FromBody] BaseIdListInput input)
    {
        await _sysOrgService.Delete(input);
    }

    #endregion Post
}
