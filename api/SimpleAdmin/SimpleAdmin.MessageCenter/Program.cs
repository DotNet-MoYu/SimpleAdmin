try
{
    Console.Title = "SimpleAdmin消息中心服务";
}
catch
{
    // ignored
}//注册服务会报错所以要catch
Serve.Run(GenericRunOptions.Default
        .ConfigureBuilder(hostBuilder =>
        {
            hostBuilder.UseWindowsService();//支持注册成服务
            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                services.AddMqttClientManager();//mqtt
            });
            return hostBuilder;
        })
    );
