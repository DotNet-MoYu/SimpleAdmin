namespace SimpleAdmin.Web.Core;
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
    [HttpGet("userSelector")]
    public async Task<dynamic> UserSelector([FromQuery] UserSelectorInput input)
    {
        return await _sysUserService.UserSelector(input);
    }

    /// <summary>
    /// 获取组织详情
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("detail")]
    public async Task<dynamic> Detail([FromBody]BaseIdInput input)
    {
        return await _sysOrgService.Detail(input); ;
    }

    #endregion

    #region Post

    /// <summary>
    /// 复制组织
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("copy")]
    [DisplayName("复制组织")]
    public async Task Copy([FromBody]SysOrgCopyInput input)
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
    public async Task Delete([FromBody] List<BaseIdInput> input)
    {
        await _sysOrgService.Delete(input);
    }
    #endregion
}
