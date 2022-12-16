using Furion.FriendlyException;
using System.Linq;

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
        var whiteAction = new string[] { "loginOut" };
        //只拦截post方法并且不是
        if (httpRequest.Method == "POST" && !whiteAction.Contains(actionDescriptor.ActionName))
        {
            // 是否匿名访问
            var allowAnonymouse = context.Filters.Any(u => u is IAllowAnonymousFilter)
                            || controllerType.IsDefined(typeof(AllowAnonymousAttribute), true)
                            || method.IsDefined(typeof(AllowAnonymousAttribute), true);
            if (!allowAnonymouse)
            {
                //如果不是匿名访问,抛出
                throw Oops.Bah("演示环境,禁止操作");
            }

        }
        await next();
    }
}
