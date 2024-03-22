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
/// 会话管理控制器
/// </summary>
[ApiDescriptionSettings(Tag = "会话管理")]
[Route("sys/auth/[controller]")]
[SuperAdmin]
public class SessionController : BaseController
{
    private readonly ISessionService _sessionService;

    public SessionController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    /// <summary>
    /// B端会话分页
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("b/page")]
    public async Task<dynamic> PageB([FromQuery] SessionPageInput input)
    {
        return await _sessionService.PageB(input);
    }

    /// <summary>
    /// C端会话分页
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("c/page")]
    public async Task<dynamic> PageC([FromQuery] SessionPageInput input)
    {
        return await _sessionService.PageC(input);
    }

    /// <summary>
    /// 会话统计
    /// </summary>
    /// <returns></returns>
    [HttpGet("analysis")]
    public dynamic Analysis()
    {
        return _sessionService.Analysis();
    }

    /// <summary>
    /// 强退B端会话
    /// </summary>
    /// <param name="input"></param>
    [HttpPost("b/exitSession")]
    [DisplayName("强退B端会话")]
    public async Task ExitSessionForB([FromBody] BaseIdInput input)
    {
        await _sessionService.ExitSession(input);
    }

    /// <summary>
    /// 强退C端会话
    /// </summary>
    /// <param name="input"></param>
    [HttpPost("c/exitSession")]
    [DisplayName("强退C端会话")]
    public async Task ExitSessionForC([FromBody] BaseIdInput input)
    {
        await _sessionService.ExitSession(input);
    }

    /// <summary>
    /// 强退B端Token
    /// </summary>
    /// <param name="input"></param>
    [HttpPost("b/ExitToken")]
    [DisplayName("强退B端Token")]
    public async Task ExitTokenForB([FromBody] ExitTokenInput input)
    {
        await _sessionService.ExitToken(input);
    }

    /// <summary>
    /// 强退C端Token
    /// </summary>
    /// <param name="input"></param>
    [HttpPost("c/ExitToken")]
    [DisplayName("强退C端Token")]
    public async Task ExitTokenForC([FromBody] ExitTokenInput input)
    {
        await _sessionService.ExitToken(input);
    }
}
