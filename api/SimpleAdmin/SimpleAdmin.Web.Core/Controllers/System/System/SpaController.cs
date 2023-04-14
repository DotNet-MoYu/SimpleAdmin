namespace SimpleAdmin.Web.Core;

/// <summary>
/// 单页管理控制器
/// </summary>
[ApiDescriptionSettings(Tag = "单页管理")]
public class SpaController : BaseController
{
    private readonly ISpaService _spaService;

    public SpaController(ISpaService spaService)
    {
        _spaService = spaService;
    }

    /// <summary>
    /// 单页分页查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("page")]
    public async Task<dynamic> Page([FromQuery] SpaPageInput input)
    {
        return await _spaService.Page(input);
    }

    /// <summary>
    /// 添加单页
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [DisplayName("添加单页")]
    public async Task Add([FromBody] SpaAddInput input)
    {
        await _spaService.Add(input);
    }

    /// <summary>
    /// 修改单页
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("edit")]
    [DisplayName("修改单页")]
    public async Task Edit([FromBody] SpaEditInput input)
    {
        await _spaService.Edit(input);
    }

    /// <summary>
    /// 删除单页
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>

    [HttpPost("delete")]
    [DisplayName("删除单页")]
    public async Task Delete([FromBody] List<BaseIdInput> input)
    {
        await _spaService.Delete(input);
    }
}