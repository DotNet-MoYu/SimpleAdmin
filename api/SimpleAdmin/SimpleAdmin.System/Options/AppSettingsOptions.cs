using Furion.ConfigurableOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.System
{
    /// <summary>
    /// 系统配置选项
    /// </summary>
    public class AppSettingsOptions : IConfigurableOptions
    {

        /// <summary>
        /// 是否演示环境
        /// </summary>
        public bool EnvPoc { get; set; } = false;

        /// <summary>
        /// 是否清除Redis缓存
        /// </summary>
        public bool ClearRedis { get; set; } = false;
    }
}
