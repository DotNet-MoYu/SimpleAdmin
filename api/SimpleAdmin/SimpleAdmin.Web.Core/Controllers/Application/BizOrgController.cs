namespace SimpleAdmin.Web.Core;

/// <summary>
/// 机构管理控制器
/// </summary>
[ApiDescriptionSettings("Application", Tag = "机构管理")]
[Route("/biz/org")]
[RolePermission]
public class BizOrgController : IDynamicApiController
{
    private readonly IOrgService _orgService;
    private readonly IUserService _userService;

    public BizOrgController(IOrgService orgService, IUserService userService)
    {
        _orgService = orgService;
        this._userService = userService;
    }

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
    [DisplayName("添加机构")]
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
    [DisplayName("修改机构")]
    public async Task Edit([FromBody] OrgEditInput input)
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
    public async Task Copy(OrgCopyInput input)
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
    public async Task Delete([FromBody] List<BaseIdInput> input)
    {
        await _orgService.Delete(input);
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
        return await _userService.UserSelector(input);
    }

}
