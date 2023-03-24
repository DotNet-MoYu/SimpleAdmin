namespace SimpleAdmin.Plugin.Mqtt;

/// <summary>
///  mqtt组件
/// </summary>
public sealed class MqttComponent : IServiceComponent
{

    public void Load(IServiceCollection services, ComponentContext componentContext)
    {
        Console.WriteLine("注册Mqtt插件");
        services.AddMqttClientManager();
    }
}

/// <summary>
/// mqtt组件
/// 模拟 Configure
/// </summary>
public sealed class MqttApplicationComponent : IApplicationComponent
{
    public void Load(IApplicationBuilder app, IWebHostEnvironment env, ComponentContext componentContext)
    {
        App.GetService<IMqttClientManager>();//获取mqtt服务判断配置是否有问题
    }
}