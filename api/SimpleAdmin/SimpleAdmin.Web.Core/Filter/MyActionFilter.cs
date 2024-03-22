// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

using MoYu.FriendlyException;

namespace SimpleAdmin.Web.Core;

/// <summary>
/// 操作筛选器
/// </summary>
public class MyActionFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // 获取控制器、路由信息
        var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

        // 获取控制器类型
        var controllerType = actionDescriptor!.ControllerTypeInfo;

        // 获取请求的方法
        var method = actionDescriptor!.MethodInfo;

        // 获取 HttpContext 和 HttpRequest 对象
        var httpContext = context.HttpContext;
        var httpRequest = httpContext.Request;
        //拦截白名单
        var whiteAction = new[] { "loginOut", "auth", "preview" };
        //只拦截post方法并且不是
        if (httpRequest.Method == "POST" && !whiteAction.Contains(actionDescriptor.ActionName))
        {
            // 是否匿名访问
            var allowAnonymous = context.Filters.Any(u => u is IAllowAnonymousFilter)
                || controllerType.IsDefined(typeof(AllowAnonymousAttribute), true) || method.IsDefined(typeof(AllowAnonymousAttribute), true);
            if (!allowAnonymous)
            {
                //如果不是匿名访问,抛出
                throw Oops.Bah("演示环境,禁止操作");
            }
        }
        await next();
    }
}
