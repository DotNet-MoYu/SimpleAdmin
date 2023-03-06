namespace SimpleAdmin.Core;

/// <summary>
/// AppStartup启动类
/// </summary>
[AppStartup(99)]
public class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddComponent<LoggingConsoleComponent>();//启动控制台日志格式化组件
        services.AddComponent<LoggingFileComponent>();//启动日志写入文件组件

        services.AddSimpleRedis();//注入Redis

        // 配置雪花Id算法机器码
        YitIdHelper.SetIdGenerator(new IdGeneratorOptions
        {
            WorkerId = 1// 取值范围0~63,默认1
        });

        //注入sqlsugar服务
        services.AddSqlSugar();
    }
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {

    }



}
