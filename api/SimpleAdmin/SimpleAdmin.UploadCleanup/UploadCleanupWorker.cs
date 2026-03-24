using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SimpleAdmin.Application;

namespace SimpleAdmin.UploadCleanup;

/// <summary>
/// 清理过期分片上传会话
/// </summary>
public class UploadCleanupWorker : BackgroundService
{
    private readonly ILogger<UploadCleanupWorker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public UploadCleanupWorker(ILogger<UploadCleanupWorker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var storageService = scope.ServiceProvider.GetRequiredService<IDocumentStorageService>();
                var cleanedCount = await storageService.CleanupExpiredChunkUploads(stoppingToken);
                if (cleanedCount > 0)
                    _logger.LogInformation("已清理过期上传会话 {Count} 个", cleanedCount);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "清理过期上传会话失败");
            }

            await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
        }
    }
}
