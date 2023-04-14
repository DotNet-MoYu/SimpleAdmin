using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace SimpleAdmin.Web.Core;

/// <summary>
/// 认证相关组件
/// </summary>
public sealed class AuthComponent : IServiceComponent
{
    public void Load(IServiceCollection services, ComponentContext componentContext)
    {
        // JWT配置
        services.AddJwt<JwtHandler>(enableGlobalAuthorize: true, jwtBearerConfigure: options =>
        {
            //signalr jwt配置
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    // If the request is for our hub...
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) &&
                        (path.StartsWithSegments("/hubs")))
                    {
                        // Read the token out of the query string
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        });
        // 允许跨域
        services.AddCorsAccessor();

        //注册自定义授权筛选器
        services.AddMvcFilter<MyAuthorizationFilter>();
    }
}