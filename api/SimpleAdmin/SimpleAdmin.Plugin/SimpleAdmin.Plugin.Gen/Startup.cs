namespace SimpleAdmin.Plugin.Gen;

/// <summary>
/// AppStartup启动类
/// </summary>
public class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        //试图引擎
        services.AddViewEngine();
    }
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {

    }

}
