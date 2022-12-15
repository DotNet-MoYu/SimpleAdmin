using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Web.Core.Controllers.Systems.Dev;

/// <summary>
/// 操作日志控制器
/// </summary>
[ApiDescriptionSettings(Tag = "操作日志")]
[Route("dev/[controller]")]
public class LogOperateController : BaseController
{
    private readonly IOperateLogService _operateLogService;

    public LogOperateController(IOperateLogService operateLogService)
    {
        _operateLogService = operateLogService;
    }

    /// <summary>
    /// 字典分页查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("page")]
    public async Task<dynamic> Page([FromQuery] OperateLogPageInput input)
    {
        return await _operateLogService.Page(input);
    }


    /// <summary>
    /// 操作日志周统计柱状图图
    /// </summary>
    /// <returns></returns>
    [HttpGet("barChartData")]
    public async Task<dynamic> BarChartData()
    {
        return await _operateLogService.StatisticsByDay(7);
    }

    /// <summary>
    /// 操作日志数量总览饼图
    /// </summary>
    /// <returns></returns>
    [HttpGet("pieChartData")]
    public async Task<dynamic> PieChartData()
    {
        return await _operateLogService.TotalCount();
    }

    /// <summary>
    /// 清空日志
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("delete")]
    public async Task Delete([FromBody] OperateLogDeleteInput input)
    {
        await _operateLogService.Delete(input.Category);
    }

}
