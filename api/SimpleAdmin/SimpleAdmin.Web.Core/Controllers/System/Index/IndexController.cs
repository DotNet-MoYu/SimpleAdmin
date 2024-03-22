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
/// 系统首页控制器
/// </summary>
[ApiDescriptionSettings(Tag = "系统首页")]
[Route("home/index")]
[RolePermission]
public class IndexController : BaseController
{
    private readonly IVisitLogService _visitLogService;
    private readonly IOperateLogService _operateLogService;
    private readonly IIndexService _indexService;

    public IndexController(IVisitLogService visitLogService, IOperateLogService operateLogService, IIndexService indexService)
    {
        _visitLogService = visitLogService;
        _operateLogService = operateLogService;
        _indexService = indexService;
    }

    /// <summary>
    /// 获取当前用户访问日志列表
    /// </summary>
    /// <returns></returns>
    [HttpGet("visLog/list")]
    [DisplayName("访问日志")]
    public async Task<dynamic> VisLogList()
    {
        return await _visitLogService.Page(new VisitLogPageInput { Account = UserManager.UserAccount });
    }

    /// <summary>
    /// 获取当前用户操作日志列表
    /// </summary>
    /// <returns></returns>
    [HttpGet("opLog/list")]
    [DisplayName("操作日志")]
    public async Task<dynamic> OpLogList()
    {
        return await _operateLogService.Page(new OperateLogPageInput { Account = UserManager.UserAccount });
    }

    /// <summary>
    /// 获取当前用户站内信列表
    /// </summary>
    /// <returns></returns>
    [HttpGet("message/list")]
    [DisplayName("站内信列表")]
    public async Task<dynamic> MessageList()
    {
        return new string[] { };
    }

    /// <summary>
    /// 获取当前用户日程列表
    /// </summary>
    /// <returns></returns>
    [HttpGet("schedule/list")]
    [DisplayName("日程列表")]
    public async Task<dynamic> ScheduleList([FromQuery] ScheduleListInput input)
    {
        return await _indexService.ScheduleList(input);
    }

    /// <summary>
    /// 添加日程
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("schedule/add")]
    [DisplayName("添加日程")]
    public async Task AddSchedule([FromBody] ScheduleAddInput input)
    {
        await _indexService.AddSchedule(input);
    }

    /// <summary>
    /// 删除日程
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("schedule/deleteSchedule")]
    [DisplayName("删除日程")]
    public async Task DeleteSchedule([FromBody] BaseIdListInput input)
    {
        await _indexService.DeleteSchedule(input);
    }
}
