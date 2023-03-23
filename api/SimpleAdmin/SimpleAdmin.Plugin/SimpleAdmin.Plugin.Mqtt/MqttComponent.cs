

namespace SimpleAdmin.Plugin.Mqtt;

/// <summary>
/// MQTT组件
/// </summary>
public sealed class MqttComponent : IServiceComponent
{
    public void Load(IServiceCollection services, ComponentContext componentContext)
    {
        services.AddMqttClientManager();
    }
}

/// <summary>
///MQTT组件
/// 模拟 Configure
/// </summary>
public sealed class MqttApplicationComponent : IApplicationComponent
{
    public void Load(IApplicationBuilder app, IWebHostEnvironment env, ComponentContext componentContext)
    {
        App.GetService<IMqttClientManager>();//获取mqtt服务去连接mqtt判断参数填的是否正确
    }
}