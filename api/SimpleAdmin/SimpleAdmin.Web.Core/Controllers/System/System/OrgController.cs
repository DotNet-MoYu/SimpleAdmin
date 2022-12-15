using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Web.Core.Controllers.Systems.System;

/// <summary>
/// 组织管理控制器
/// </summary>
[ApiDescriptionSettings(Tag = "组织管理")]
public class OrgController : BaseController
{
    private readonly ISysOrgService _sysOrgService;
    private readonly ISysUserService _sysUserService;

    public OrgController(ISysOrgService sysOrgService, ISysUserService sysUserService)
    {
        _sysOrgService = sysOrgService;
        this._sysUserService = sysUserService;
    }

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
    public async Task<dynamic> Page([FromQuery] OrgPageInput input)
    {
        return await _sysOrgService.Page(input);
    }

    /// <summary>
    /// 添加组织
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [Description("添加组织")]
    public async Task Add([FromBody] OrgAddInput input)
    {
        await _sysOrgService.Add(input);
    }

    /// <summary>
    /// 修改组织
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("edit")]
    [Description("修改组织")]
    public async Task Edit([FromBody] OrgEditInput input)
    {
        await _sysOrgService.Edit(input);
    }

    /// <summary>
    /// 删除组织
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>

    [HttpPost("delete")]
    [Description("删除组织")]
    public async Task Delete([FromBody] List<BaseIdInput> input)
    {
        await _sysOrgService.Delete(input);
    }

    /// <summary>
    /// 获取组织详情
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("detail")]
    public async Task<dynamic> Detail(BaseIdInput input)
    {
        return await _sysOrgService.Detail(input); ;
    }

    /// <summary>
    /// 获取用户选择器
    /// </summary>
    /// <returns></returns>
    [HttpGet("userSelector")]
    public async Task<dynamic> UserSelector([FromQuery] UserSelectorInput input)
    {
        return await _sysUserService.UserSelector(input);
    }
}
