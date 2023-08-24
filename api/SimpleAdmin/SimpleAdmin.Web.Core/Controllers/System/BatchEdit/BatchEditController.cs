namespace SimpleAdmin.Web.Core;

/// <summary>
/// 批量控制器
/// </summary>
[ApiDescriptionSettings(Tag = "批量修改")]
[Route("/sys/batch")]
public class BatchEditController : BaseController
{
    private readonly IBatchEditService _batchEditService;

    public BatchEditController(IBatchEditService updateBatchService)
    {
        _batchEditService = updateBatchService;
    }

    #region Get请求

    /// <summary>
    /// 批量分页查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("page")]
    public async Task<dynamic> Page([FromQuery] BatchEditPageInput input)
    {
        return await _batchEditService.Page(input);
    }

    /// <summary>
    /// 获取表信息
    /// </summary>
    /// <returns></returns>
    [HttpGet("tables")]
    public dynamic Tables()
    {
        return _batchEditService.GetTables();
    }

    /// <summary>
    /// 获取批量配置信息
    /// </summary>
    /// <returns></returns>
    [HttpGet("configs")]
    public async Task<dynamic> ConfigList([FromQuery] BaseIdInput input)
    {
        return await _batchEditService.ConfigList(input);
    }

    /// <summary>
    /// 获取批量配置信息
    /// </summary>
    /// <returns></returns>
    [HttpGet("columns")]
    [IgnoreSuperAdmin]
    public async Task<dynamic> Columns([FromQuery] string code)
    {
        return await _batchEditService.Columns(code);
    }

    #endregion Get请求

    #region Post请求

    /// <summary>
    /// 添加批量
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [DisplayName("添加批量更新")]
    public async Task Add([FromBody] BatchEditAddInput input)
    {
        await _batchEditService.Add(input);
    }

    /// <summary>
    /// 配置修改批量
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("config")]
    [DisplayName("配置批量更新")]
    public async Task Config([FromBody] List<BatchEditConfigInput> input)
    {
        await _batchEditService.Config(input);
    }

    /// <summary>
    /// 删除批量
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("delete")]
    [DisplayName("删除批量更新")]
    public async Task Delete([FromBody] BaseIdListInput input)
    {
        await _batchEditService.Delete(input);
    }

    /// <summary>
    /// 批量更新同步
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("sync")]
    [DisplayName("批量更新同步")]
    public async Task Sync([FromBody] BaseIdInput input)
    {
        await _batchEditService.SyncColumns(input);
    }

    #endregion Post请求
}