namespace SimpleAdmin.Web.Core;

/// <summary>
/// 岗位管理控制器
/// </summary>
[ApiDescriptionSettings("Application", Tag = "岗位管理")]
[Route("/biz/position")]
[RolePermission]
public class BizPositionController
{
    private readonly IPositionService _positionService;

    public BizPositionController(IPositionService positionService)
    {
        this._positionService = positionService;
    }

    /// <summary>
    /// 岗位分页查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("page")]
    [DisplayName("岗位分页查询")]
    public async Task<dynamic> Page([FromQuery] PositionPageInput input)
    {
        return await _positionService.Page(input);
    }

    /// <summary>
    /// 添加岗位
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [DisplayName("添加岗位")]
    public async Task Add([FromBody] PositionAddInput input)
    {
        await _positionService.Add(input);
    }

    /// <summary>
    /// 修改岗位
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("edit")]
    [DisplayName("修改岗位")]
    public async Task Edit([FromBody] PositionEditInput input)
    {
        await _positionService.Edit(input);
    }

    /// <summary>
    /// 删除岗位
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>

    [HttpPost("delete")]
    [DisplayName("删除岗位")]
    public async Task Delete([FromBody] List<BaseIdInput> input)
    {
        await _positionService.Delete(input);
    }
}