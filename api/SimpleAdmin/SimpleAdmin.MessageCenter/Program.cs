try
{
    Console.Title = "SimpleAdmin��Ϣ���ķ���";
}
catch
{
    // ignored
}//ע�����ᱨ������Ҫcatch
Serve.Run(GenericRunOptions.Default
        .ConfigureBuilder(hostBuilder =>
        {
            hostBuilder.UseWindowsService();//֧��ע��ɷ���
            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                services.AddMqttClientManager();//mqtt
            });
            return hostBuilder;
        })
    );
