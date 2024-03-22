// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.Core;

/// <summary>
/// 规范化RESTful风格返回值
/// </summary>
[SuppressSniffer]
[UnifyModel(typeof(SimpleAdminResult<>))]
public class SimpleAdminResultProvider : IUnifyResultProvider
{
    /// <summary>
    /// 异常返回
    /// </summary>
    /// <param name="context"></param>
    /// <param name="metadata"></param>
    /// <returns></returns>
    public IActionResult OnException(ExceptionContext context, ExceptionMetadata metadata)
    {
        return new JsonResult(ResTfulResult(metadata.StatusCode, data: metadata.Data, errors: metadata.Errors));
    }

    /// <summary>
    /// 成功返回
    /// </summary>
    /// <param name="context"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public IActionResult OnSucceeded(ActionExecutedContext context, object data)
    {
        return new JsonResult(ResTfulResult(StatusCodes.Status200OK, true, data));
    }

    /// <summary>
    /// 验证失败返回
    /// </summary>
    /// <param name="context"></param>
    /// <param name="metadata"></param>
    /// <returns></returns>
    public IActionResult OnValidateFailed(ActionExecutingContext context, ValidationMetadata metadata)
    {
        return new JsonResult(ResTfulResult(metadata.StatusCode ?? StatusCodes.Status400BadRequest, data: metadata.Data,
            errors: metadata.FirstErrorMessage ?? metadata.Message));
    }

    /// <summary>
    /// 状态码响应拦截
    /// </summary>
    /// <param name="context"></param>
    /// <param name="statusCode"></param>
    /// <param name="unifyResultSettings"></param>
    /// <returns></returns>
    public async Task OnResponseStatusCodes(HttpContext context, int statusCode, UnifyResultSettingsOptions unifyResultSettings = null)
    {
        // 设置响应状态码
        UnifyContext.SetResponseStatusCodes(context, statusCode, unifyResultSettings);

        switch (statusCode)
        {
            // 处理 401 状态码
            case StatusCodes.Status401Unauthorized:
                await context.Response.WriteAsJsonAsync(ResTfulResult(statusCode, errors: "登录已过期，请重新登录"),
                    App.GetOptions<JsonOptions>()?.JsonSerializerOptions);
                break;
            // 处理 403 状态码
            case StatusCodes.Status403Forbidden:
                await context.Response.WriteAsJsonAsync(ResTfulResult(statusCode, errors: "禁止访问，没有权限", data: context.Request.Path),
                    App.GetOptions<JsonOptions>()?.JsonSerializerOptions);
                break;
        }
    }

    /// <summary>
    /// 返回 RESTful 风格结果集
    /// </summary>
    /// <param name="statusCode">状态码</param>
    /// <param name="succeeded">是否成功</param>
    /// <param name="data">数据</param>
    /// <param name="errors">错误信息</param>
    /// <returns></returns>
    private static SimpleAdminResult<object> ResTfulResult(int statusCode, bool succeeded = default, object data = default,
        object errors = default)
    {
        return new SimpleAdminResult<object>
        {
            Code = statusCode,
            Msg = statusCode == StatusCodes.Status200OK ? "请求成功" : errors,
            Data = data,
            Extras = UnifyContext.Take(),
            Time = DateTime.Now
        };
    }
}
