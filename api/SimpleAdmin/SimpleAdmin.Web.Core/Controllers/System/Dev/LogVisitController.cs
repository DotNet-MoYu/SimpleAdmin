namespace SimpleAdmin.Web.Core;

/// <summary>
/// 访问日志控制器
/// </summary>
[ApiDescriptionSettings(Tag = "访问日志")]
[Route("dev/[controller]")]
public class LogVisitController : BaseController
{
    private readonly IVisitLogService _visitLogService;

    public LogVisitController(IVisitLogService visitLogService)
    {
        _visitLogService = visitLogService;
    }

    /// <summary>
    /// 字典分页查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("page")]
    public async Task<dynamic> Page([FromQuery] VisitLogPageInput input)
    {
        return await _visitLogService.Page(input);
    }

    /// <summary>
    /// 访问日志周统计折线图
    /// </summary>
    /// <returns></returns>
    [HttpGet("lineChartData")]
    public async Task<dynamic> LineChartData()
    {
        return await _visitLogService.StatisticsByDay(7);
    }

    /// <summary>
    /// 访问日志总览饼图
    /// </summary>
    /// <returns></returns>
    [HttpGet("pieChartData")]
    public async Task<dynamic> PieChartData()
    {
        return await _visitLogService.TotalCount();
    }

    /// <summary>
    /// 清空日志
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("delete")]
    public async Task Delete([FromBody] VisitLogDeleteInput input)
    {
        await _visitLogService.Delete(input.Category);
    }
}