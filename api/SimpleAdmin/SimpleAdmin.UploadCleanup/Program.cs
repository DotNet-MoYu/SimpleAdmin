using SimpleAdmin.UploadCleanup;

try
{
    Console.Title = "SimpleAdmin上传清理服务";
}
catch
{
    // ignored
}

Serve.Run(GenericRunOptions.Default
        .ConfigureBuilder(hostBuilder =>
        {
            hostBuilder.UseWindowsService();
            hostBuilder.ConfigureServices((_, services) =>
            {
                services.AddHostedService<UploadCleanupWorker>();
            });
            return hostBuilder;
        })
    );
