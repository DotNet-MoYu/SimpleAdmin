
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using SimpleMQTT;

namespace SimpleAdmin.System;

/// <summary>
/// AppStartup启动类
/// </summary>
public class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        //事件总线
        services.AddEventBus();
        //试图引擎
        services.AddViewEngine();
    }
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {

    }

}
