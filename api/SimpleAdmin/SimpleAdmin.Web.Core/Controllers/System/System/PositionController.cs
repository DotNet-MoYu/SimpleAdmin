namespace SimpleAdmin.Web.Core;
/// <summary>
/// 职位管理控制器
/// </summary>
[ApiDescriptionSettings(Tag = "职位管理")]
public class PositionController : BaseController
{
    private readonly ISysPositionService _sysPositionService;

    public PositionController(ISysPositionService sysPositionService)
    {
        this._sysPositionService = sysPositionService;
    }

    /// <summary>
    /// 职位分页查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("page")]
    [Description("职位分页查询")]
    public async Task<dynamic> Page([FromQuery] PositionPageInput input)
    {
        return await _sysPositionService.Page(input);
    }

    /// <summary>
    /// 添加职位
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [Description("添加职位")]
    public async Task Add([FromBody] PositionAddInput input)
    {
        await _sysPositionService.Add(input);
    }

    /// <summary>
    /// 修改职位
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("edit")]
    [Description("修改职位")]
    public async Task Edit([FromBody] PositionEditInput input)
    {
        await _sysPositionService.Edit(input);
    }

    /// <summary>
    /// 删除职位
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>

    [HttpPost("delete")]
    [Description("删除职位")]
    public async Task Delete([FromBody] List<BaseIdInput> input)
    {
        await _sysPositionService.Delete(input);
    }


}
