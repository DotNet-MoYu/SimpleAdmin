using SimpleAdmin.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Web.Core.Controllers.Application;

/// <summary>
/// 机构管理控制器
/// </summary>
[ApiDescriptionSettings("Application", Tag = "机构管理")]
[Route("/biz/org")]
[RolePermission]
public class OrgController : IDynamicApiController
{
    private readonly IOrgService _orgService;
    private readonly IUserService _userService;

    public OrgController(IOrgService orgService, IUserService userService)
    {
        _orgService = orgService;
        this._userService = userService;
    }

    /// <summary>
    /// 获取机构树
    /// </summary>
    /// <returns></returns>
    [HttpGet("tree")]
    [Description("机构树查询")]
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
    [Description("机构分页查询")]
    public async Task<dynamic> Page([FromQuery] OrgPageInput input)
    {
        return await _orgService.Page(input);
    }

    /// <summary>
    /// 添加机构
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [Description("添加机构")]
    public async Task Add([FromBody] OrgAddInput input)
    {
        await _orgService.Add(input);
    }

    /// <summary>
    /// 修改机构
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("edit")]
    [Description("修改机构")]
    public async Task Edit([FromBody] OrgEditInput input)
    {
        await _orgService.Edit(input);
    }

    /// <summary>
    /// 删除机构
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>

    [HttpPost("delete")]
    [Description("删除机构")]
    public async Task Delete([FromBody] List<BaseIdInput> input)
    {
        await _orgService.Delete(input);
    }

    /// <summary>
    /// 获取机构树选择器
    /// </summary>
    /// <returns></returns>
    [HttpGet("orgTreeSelector")]
    [Description("机构树选择器")]
    public async Task<dynamic> OrgTreeSelector()
    {
        return await _orgService.Tree();
    }


    /// <summary>
    /// 获取人员选择器
    /// </summary>
    /// <returns></returns>
    [HttpGet("userSelector")]
    [Description("人员选择器")]
    public async Task<dynamic> UserSelector([FromQuery] UserSelectorInput input)
    {
        return await _userService.UserSelector(input);
    }

}
