using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Web.Core.Controllers.System.Dev
{
    /// <summary>
    /// 站内信控制器
    /// </summary>
    [ApiDescriptionSettings(Tag = "站内信")]
    [Route("dev/[controller]")]
    public class MessageController : BaseController
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            this._messageService = messageService;
        }


        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("page")]
        public async Task<dynamic> Page([FromQuery] MessagePageInput input)
        {
            return await _messageService.Page(input);
        }

        /// <summary>
        /// 发送站内信
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("send")]
        [Description("发送站内信")]
        public async Task Send(MessageSendInput input)
        {
            await _messageService.Send(input);
        }
    }
}
