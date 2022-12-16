using Masuit.Tools.DateTimeExt;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Shiny.Helper;
using Shiny.Helper.Helper;

namespace SimpleAdmin.System;


/// <summary>
/// AppStartup启动类
/// </summary>
public class Startup : AppStartup
{

    public void ConfigureServices(IServiceCollection services)
    {
        var a = 2.0 / 5;
        var b = 1 - a;
        services.AddComponent<LoggingMonitorComponent>();//启动LoggingMonitor操作日志写入数据库组件


    }
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {

    }
}
