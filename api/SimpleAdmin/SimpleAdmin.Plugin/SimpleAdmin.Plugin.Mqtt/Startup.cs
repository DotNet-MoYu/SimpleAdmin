
using SimpleMQTT;

namespace SimpleAdmin.Plugin.Mqtt;

/// <summary>
/// AppStartup启动类
/// </summary>
public class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMqttClientManager();
    }
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        var mqttClientManager = App.GetService<IMqttClientManager>();//获取mqtt服务判断配置是否有问题
    }

}
