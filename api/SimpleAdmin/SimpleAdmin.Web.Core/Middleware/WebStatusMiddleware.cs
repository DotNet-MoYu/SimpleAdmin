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
///  网站开启状态 状态中间件
/// </summary>
public class WebStatusMiddleware
{
    private readonly RequestDelegate _next;

    public WebStatusMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value;//获取请求路径
        // 检查请求路径是否以 "/biz" 开头
        if (path.Length > 1 && !path.Contains('.') && !path.StartsWith("/sys", StringComparison.OrdinalIgnoreCase))
        {
            // 通过 context.RequestServices 解析
            var configService = context.RequestServices.GetService<IConfigService>();
            // 获取网站状态
            var webStatus = await configService.GetByConfigKey(CateGoryConst.CONFIG_SYS_BASE, SysConfigConst.SYS_WEB_STATUS);
            // 如果网站状态为禁用，则返回 443 状态码
            if (webStatus.ConfigValue == CommonStatusConst.DISABLED)
            {
                context.Response.StatusCode = 423;
                return;
            }
        }
        await _next(context);
    }
}

/// <summary>
/// 中间件拓展类
/// </summary>
public static class WebStatusMiddlewareExtensions
{
    public static IApplicationBuilder UseWebStatus(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<WebStatusMiddleware>();
    }
}
