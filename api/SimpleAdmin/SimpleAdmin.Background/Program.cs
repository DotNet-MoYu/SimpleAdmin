global using Furion;
global using SimpleMQTT;
global using SimpleRedis;
try
{
    Console.Title = "SimpleAdmin��̨����";
}
catch { }//ע�����ᱨ������Ҫcatch
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


