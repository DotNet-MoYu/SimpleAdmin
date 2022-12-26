

using SimpleRedis;
using System.Security.Claims;

namespace SimpleAdmin.Web.Core;

/// <summary>
/// 自定义授权筛选器
/// </summary>
public class MyAuthorizationFilter : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // 获取控制器信息
        var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

        // 获取控制器类型
        var controllerType = actionDescriptor!.ControllerTypeInfo;

        // 获取 Action 类型
        var methodType = actionDescriptor.MethodInfo;
        // 是否匿名访问
        var allowAnonymouse = context.Filters.Any(u => u is IAllowAnonymousFilter)
                        || controllerType.IsDefined(typeof(AllowAnonymousAttribute), true)
                        || methodType.IsDefined(typeof(AllowAnonymousAttribute), true);

        // 不是匿名才处理权限检查
        if (!allowAnonymouse)
        {
            // 获取 HttpContext 和 HttpRequest 对象
            var httpContext = context.HttpContext;
            if (httpContext.Response.StatusCode == 401)
            {
                // 返回未授权
                context.Result = new UnauthorizedResult();
            }
        }
        else await Task.CompletedTask; // 否则直接跳过处理
    }
}
