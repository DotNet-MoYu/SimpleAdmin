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
            // Configure the Authority to the expected value for
            // the authentication provider. This ensures the token
            // is appropriately validated.
            //options.Authority = "Authority URL"; // TODO: Update URL

            // We have to hook the OnMessageReceived event in order to
            // allow the JWT authentication handler to read the access
            // token from the query string when a WebSocket or 
            // Server-Sent Events request comes in.

            // Sending the access token in the query string is required due to
            // a limitation in Browser APIs. We restrict it to only calls to the
            // SignalR hub in this code.
            // See https://docs.microsoft.com/aspnet/core/signalr/security#access-token-logging
            // for more information about security considerations when using
            // the query string to transmit the access token.
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
