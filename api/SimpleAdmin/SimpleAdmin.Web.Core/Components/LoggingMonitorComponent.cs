// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

using UAParser;

namespace SimpleAdmin.Web.Core;

/// <summary>
/// LoggingMonitor操作日志写入数据库插件
/// </summary>
public sealed class LoggingMonitorComponent : IServiceComponent
{
    public void Load(IServiceCollection services, ComponentContext componentContext)
    {
        //Monitor日志配置
        services.AddMonitorLogging(options =>
        {
            options.JsonIndented = true;// 是否美化 JSON
            options.GlobalEnabled = true;//全局启用
            options.ConfigureLogger((logger, logContext, context) =>
            {
                var httpContext = context.HttpContext;//获取httpContext
                //获取头
                var userAgent = httpContext.Request.Headers["User-Agent"];
                if (string.IsNullOrEmpty(userAgent)) userAgent = "Other";//如果没有这个头就指定一个
                //获取客户端信息
                var client = Parser.GetDefault().Parse(userAgent);
                // 获取控制器/操作描述器
                // var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
                //操作名称默认是控制器名加方法名,自定义操作名称要在action上加Description特性
                //var option = $"{controllerActionDescriptor.ControllerName}/{controllerActionDescriptor.ActionName}";
                ////获取特性
                //var monitor = controllerActionDescriptor.MethodInfo.GetCustomAttribute<DescriptionAttribute>();
                //if (monitor != null)//如果有LoggingMonitor特性
                //    option = monitor.Description;//则将操作名称赋值为控制器上写的title
                //logContext.Set(LoggingConst.Operation, option);//传操作名称
                logContext.Set(LoggingConst.CLIENT, client);//客户端信息
                logContext.Set(LoggingConst.PATH, httpContext.Request.Path.Value);//请求地址
                logContext.Set(LoggingConst.METHOD, httpContext.Request.Method);//请求方法
            });
        });
        //日志写入数据库配置
        services.AddDatabaseLogging<DatabaseLoggingWriter>(options =>
        {
            options.WriteFilter = logMsg =>
            {
                return logMsg.LogName == "System.Logging.LoggingMonitor";//只写入LoggingMonitor日志
            };
        });
    }
}
