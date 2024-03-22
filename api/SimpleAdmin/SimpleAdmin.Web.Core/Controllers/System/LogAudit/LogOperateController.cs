// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.Web.Core;

/// <summary>
/// 操作日志控制器
/// </summary>
[ApiDescriptionSettings(Tag = "操作日志")]
[Route("sys/audit/[controller]")]
[SuperAdmin]
public class LogOperateController : BaseController
{
    private readonly IOperateLogService _operateLogService;

    public LogOperateController(IOperateLogService operateLogService)
    {
        _operateLogService = operateLogService;
    }

    /// <summary>
    /// 操作日志分页查询
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
    [HttpGet("columnChartData")]
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

    /// <summary>
    /// 日志详情
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("detail")]
    public async Task<dynamic> Detail([FromQuery] BaseIdInput input)
    {
        return await _operateLogService.Detail(input);
    }
}
