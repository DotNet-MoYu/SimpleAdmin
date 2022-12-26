using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.System
{
    /// <summary>
    /// mqtt认证参数
    /// </summary>
    public class MqttAuthInput
    {

        /// <summary>
        /// 账号
        /// </summary>
        public string Username { get; set; }


        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }


        /// <summary>
        /// 客户端ID
        /// </summary>
        public string ClientId { get; set; }
    }
}
