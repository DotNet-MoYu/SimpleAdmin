namespace SimpleAdmin.Plugin.Mqtt;

/// <summary>
///  mqtt组件
/// </summary>
public sealed class MqttComponent : IServiceComponent
{
    /// <summary>
    /// ConfigureServices中不能解析服务，比如App.GetService()，尤其是不能在ConfigureServices中获取诸如缓存等数据进行初始化，应该在Configure中进行
    /// 服务都还没初始化完成，会导致内存中存在多份 IOC 容器！！
    /// 正确应该在 Configure 中，这个时候服务（IServiceCollection 已经完成 BuildServiceProvider() 操作了
    /// </summary>
    /// <param name="services"></param>
    /// <param name="componentContext"></param>
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