namespace SimpleAdmin.Plugin.SignalR;

/// <summary>
///  SignalR组件
/// </summary>
public sealed class SignalRComponent : IServiceComponent
{

    public void Load(IServiceCollection services, ComponentContext componentContext)
    {
        Console.WriteLine("注册SignalR插件");
        services.AddSignalR();//注册SignalR
        services.AddSingleton<IUserIdProvider, UserIdProvider>();//用户ID提供器
    }
}
