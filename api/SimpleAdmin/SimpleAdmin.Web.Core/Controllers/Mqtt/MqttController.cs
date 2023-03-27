using SimpleAdmin.Plugin.Mqtt;

namespace SimpleAdmin.Web.Core.Controllers.Mqtt;

/// <summary>
/// mqtt服务控制器
/// </summary>
[ApiDescriptionSettings(Tag = "mqtt服务")]
[Route("mqtt")]
public class MqttController
{
    private readonly IMqttService _mqttService;
    private readonly ISysUserService _sysUserService;

    public MqttController(IMqttService mqttService, ISysUserService sysUserService)
    {
        _mqttService = mqttService;
        _sysUserService = sysUserService;
    }

    /// <summary>
    /// 获取mqtt登录参数
    /// </summary>
    /// <returns></returns>
    [HttpGet("getParameter")]
    public async Task<dynamic> GetParameter()
    {
        return await _mqttService.GetWebLoginParameter(await _sysUserService.GetUserById(UserManager.UserId));
    }

    /// <summary>
    /// mqtt认证
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("auth")]
    [AllowAnonymous]
    [NonUnify]
    public async Task<dynamic> Auth([FromBody] MqttAuthInput input)
    {
        var user = await _sysUserService.GetUserByAccount(input.Username);
        if (user != null)
            return await _mqttService.Auth(input, user.Id.ToString());
        else
            return new MqttAuthOutput { };

    }
}
