namespace SimpleAdmin.Web.Core;
/// <summary>
/// 系统首页控制器
/// </summary>
[ApiDescriptionSettings(Tag = "系统首页")]
[Route("sys/[controller]")]
public class IndexController : IDynamicApiController
{
    private readonly IVisitLogService _visitLogService;
    private readonly IOperateLogService _operateLogService;
    private readonly IIndexService _indexService;

    public IndexController(IVisitLogService visitLogService, IOperateLogService operateLogService, IIndexService indexService)
    {
        this._visitLogService = visitLogService;
        this._operateLogService = operateLogService;
        this._indexService = indexService;
    }

    /// <summary>
    /// 获取当前用户访问日志列表
    /// </summary>
    /// <returns></returns>

    [HttpGet("visLog/list")]
    public async Task<dynamic> VisLogList()
    {

        return await _visitLogService.Page(new VisitLogPageInput { Account = UserManager.UserAccount });
    }

    /// <summary>
    /// 获取当前用户操作日志列表
    /// </summary>
    /// <returns></returns>
    [HttpGet("opLog/list")]
    public async Task<dynamic> OpLogList()
    {
        return await _operateLogService.Page(new OperateLogPageInput { Account = UserManager.UserAccount });
    }

    /// <summary>
    /// 获取当前用户站内信列表
    /// </summary>
    /// <returns></returns>
    [HttpGet("message/list")]
    public async Task<dynamic> MessageList()
    {
        return new string[] { };
    }

    /// <summary>
    /// 获取当前用户日程列表
    /// </summary>
    /// <returns></returns>
    [HttpGet("schedule/list")]
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
    [Description("添加日程")]
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
    [Description("删除日程")]
    public async Task DeleteSchedule([FromBody] List<BaseIdInput> input)
    {
        await _indexService.DeleteSchedule(input);
    }
}
