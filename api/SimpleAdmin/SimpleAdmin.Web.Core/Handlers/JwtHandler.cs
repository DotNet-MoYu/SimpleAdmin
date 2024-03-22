// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

using MoYu.Authorization;
using MoYu.DataEncryption;
using MoYu.Logging.Extensions;
using System.Security.Claims;

namespace SimpleAdmin.Web.Core;

public class JwtHandler : AppAuthorizeHandler
{
    /// <summary>
    ///     重写 Handler 添加自动刷新
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var expire = App.GetConfig<int>("JWTSettings:ExpiredTime");//获取过期时间(分钟)
        var currentHttpContext = context.GetCurrentHttpContext();
        //自动刷新Token
        if (JWTEncryption.AutoRefreshToken(context, currentHttpContext, expire, expire * 2))
        {
            //判断token是否有效
            if (CheckTokenFromRedis(currentHttpContext, expire))
            {
                await AuthorizeHandleAsync(context);
            }
            else
            {
                currentHttpContext.Response.StatusCode = 401;//返回401给授权筛选器用
            }
        }
        else
        {
            context.Fail();// 授权失败
            if (currentHttpContext == null)
                return;
            currentHttpContext.SignoutToSwagger();
        }
    }

    /// <summary>
    ///     检查token有效性
    /// </summary>
    /// <param name="context">DefaultHttpContext</param>
    /// <param name="expire">token有效期/分钟</param>
    /// <returns></returns>
    private bool CheckTokenFromRedis(DefaultHttpContext context, int expire)
    {
        var token = JWTEncryption.GetJwtBearerToken(context);//获取当前token
        var userId = App.User?.FindFirstValue(ClaimConst.USER_ID);//获取用户ID
        var simpleCacheService = App.GetService<ISimpleCacheService>();//获取redis实例
        var tokenInfos = simpleCacheService.HashGetOne<List<TokenInfo>>(CacheConst.CACHE_USER_TOKEN, userId);//获取token信息
        if (tokenInfos == null)//如果还是空
        {
            return false;
        }

        var tokenInfo = tokenInfos.Where(it => it.Token == token).FirstOrDefault();//获取redis中token值是当前token的对象
        if (tokenInfo != null)
        {
            // 自动刷新token返回新的Token
            var accessToken = context.Response.Headers["access-token"].ToString();
            if (!string.IsNullOrEmpty(accessToken))//如果有新的刷新token
            {
                "返回新的刷新token".LogDebug<JwtHandler>();
                tokenInfo.Token = accessToken;//新的token
                tokenInfo.TokenTimeout = DateTime.Now.AddMinutes(expire);//新的过期时间
                simpleCacheService.HashAdd(CacheConst.CACHE_USER_TOKEN, userId, tokenInfos);//更新token信息到redis
            }
        }
        else
        {
            return false;
        }

        return true;
    }

    /// <summary>
    ///     授权判断逻辑，授权通过返回 true，否则返回 false
    /// </summary>
    /// <param name="context"></param>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public override async Task<bool> PipelineAsync(AuthorizationHandlerContext context, DefaultHttpContext httpContext)
    {
        // 这里写您的授权判断逻辑，授权通过返回 true，否则返回 false

        // 此处已经自动验证 Jwt Token的有效性了，无需手动验证
        return await CheckAuthorizationAsync(httpContext);
    }

    /// <summary>
    ///     检查权限
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    private static async Task<bool> CheckAuthorizationAsync(DefaultHttpContext httpContext)
    {
        //超级管理员都能访问
        if (UserManager.SuperAdmin) return true;
        // 获取超级管理员特性
        var isSuperAdmin = httpContext.GetMetadata<SuperAdminAttribute>();
        if (isSuperAdmin != null)//如果是超级管理员才能访问的接口
        {
            //获取忽略超级管理员特性
            var ignoreSuperAdmin = httpContext.GetMetadata<IgnoreSuperAdminAttribute>();
            if (ignoreSuperAdmin == null && !UserManager.SuperAdmin)//如果只能超级管理员访问并且用户不是超级管理员
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
                var userInfo = await App.GetService<ISysUserService>().GetUserById(UserManager.UserId);
                if (!userInfo.PermissionCodeList.Contains(routeName))//如果当前路由信息不包含在角色授权路由列表中则认证失败
                    return false;
            }
        }

        return true;
    }
}
