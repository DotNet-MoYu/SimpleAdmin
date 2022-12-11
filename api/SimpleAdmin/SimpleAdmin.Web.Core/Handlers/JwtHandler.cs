using System;
using Furion;
using Furion.Authorization;
using Furion.DataEncryption;
using Microsoft.AspNetCore.Http;

namespace SimpleAdmin.Web.Core;

public class JwtHandler : AppAuthorizeHandler
{
    /// <summary>
    /// 重写 Handler 添加自动刷新
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var expire = App.GetConfig<int>("JWTSettings:ExpiredTime");//获取过期时间(分钟)
        // 自动刷新Token
        if (JWTEncryption.AutoRefreshToken(context, context.GetCurrentHttpContext(), expire, expire * 2))
        {
            await AuthorizeHandleAsync(context);
        }
        else
        {
            context.Fail(); // 授权失败
            DefaultHttpContext currentHttpContext = context.GetCurrentHttpContext();
            if (currentHttpContext == null)
                return;
            currentHttpContext.SignoutToSwagger();
        }

    }

    /// <summary>
    /// 授权判断逻辑，授权通过返回 true，否则返回 false
    /// </summary>
    /// <param name="context"></param>
    /// <param name="httpContext"></param>
    /// <returns></returns>

    public async override Task<bool> PipelineAsync(AuthorizationHandlerContext context, DefaultHttpContext httpContext)
    {
        // 这里写您的授权判断逻辑，授权通过返回 true，否则返回 false
        // 此处已经自动验证 Jwt Token的有效性了，无需手动验证
        return await CheckAuthorizationAsync(httpContext);
    }

    /// <summary>
    /// 检查权限
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    private static async Task<bool> CheckAuthorizationAsync(DefaultHttpContext httpContext)
    {
        //超级管理员都能访问
        if (UserManager.SuperAdmin) return true;
        // 获取超级管理员特性
        var isSpuerAdmin = httpContext.GetMetadata<SuperAdminAttribute>();
        if (isSpuerAdmin != null)//如果是超级管理员才能访问的接口
        {
            //获取忽略超级管理员特性
            var ignoreSpuerAdmin = httpContext.GetMetadata<IgnoreSuperAdminAttribute>();
            if (ignoreSpuerAdmin == null && !UserManager.SuperAdmin)//如果只能超级管理员访问并且用户不是超级管理员
                return false;//直接没权限
        }
        //获取角色授权特性
        var isRolePermission = httpContext.GetMetadata<RolePermissionAttribute>();
        if (isRolePermission != null)
        {
            //获取忽略角色授权特性
            var ignoreRolePermission = httpContext.GetMetadata<IgnoreRolePermissionAttribute>();
            if (ignoreRolePermission == null)
            {
                // 路由名称
                var routeName = httpContext.Request.Path.Value;
                //获取用户信息
                var userInfo = await App.GetService<ISysUserService>().GetUsertById(UserManager.UserId);
                if (userInfo != null)
                {
                    if (!userInfo.PermissionCodeList.Contains(routeName))//如果当前路由信息不包含在角色授权路由列表中则认证失败
                        return false;
                }
                else
                {
                    return false;//没有用户信息则返回认证失败
                }
            }

        }
        return true;
    }
}