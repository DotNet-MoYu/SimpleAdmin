using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Web.Core.Controllers.System.Auth
{
    /// <summary>
    /// mqtt服务控制器
    /// </summary>
    [ApiDescriptionSettings(Tag = "mqtt服务")]
    [Route("mqtt")]
    public class MqttController
    {
        private readonly IMqttService _mqttService;

        public MqttController(IMqttService mqttService)
        {
            this._mqttService = mqttService;
        }

        /// <summary>
        /// 获取mqtt登录参数
        /// </summary>
        /// <returns></returns>
        [HttpGet("getParameter")]
        public async Task<dynamic> GetParameter()
        {
            return await _mqttService.GetParameter();
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
            return await _mqttService.Auth(input);
        }
    }
}
