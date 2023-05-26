using System.Reflection;

namespace SimpleAdmin.Plugin.Gen;

/// <summary>
/// AppStartup启动类
/// </summary>
public class Startup : AppStartup
{
    /// <summary>
    /// ConfigureServices中不能解析服务，比如App.GetService()，尤其是不能在ConfigureServices中获取诸如缓存等数据进行初始化，应该在Configure中进行
    /// 服务都还没初始化完成，会导致内存中存在多份 IOC 容器！！
    /// 正确应该在 Configure 中，这个时候服务（IServiceCollection 已经完成 BuildServiceProvider() 操作了
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureServices(IServiceCollection services)
    {
        //代码生成器配置转实体
        services.AddConfigurableOptions<GenSettingsOptions>();
        //试图引擎
        services.AddViewEngine();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        var fullName = Assembly.GetExecutingAssembly().FullName;//获取程序集全名
        //通过 App.GetOptions<TOptions> 获取选项
        var settings = App.GetOptions<GenSettingsOptions>();
        CodeFirstUtils.CodeFirst(settings, fullName);//CodeFirst
    }
}