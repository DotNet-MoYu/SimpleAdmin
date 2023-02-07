namespace SimpleAdmin.Web.Core.Controllers.System.Auth;

/// <summary>
/// 会话管理控制器
/// </summary>
[ApiDescriptionSettings(Tag = "会话管理")]
[Route("auth/[controller]")]
[SuperAdmin]
public class SessionController
{
    private readonly ISessionService _sessionService;

    public SessionController(ISessionService sessionService)
    {
        this._sessionService = sessionService;
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
