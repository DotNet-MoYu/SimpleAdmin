using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Web.Core
{
    /// <summary>
    /// 认证相关组件
    /// </summary>
    public sealed class AuthComponent : IServiceComponent
    {
        public void Load(IServiceCollection services, ComponentContext componentContext)
        {
            // JWT配置
            services.AddJwt<JwtHandler>(enableGlobalAuthorize: true);
            // 允许跨域
            services.AddCorsAccessor();

            //注册自定义授权筛选器
            services.AddMvcFilter<MyAuthorizationFilter>();

        }
    }
}
