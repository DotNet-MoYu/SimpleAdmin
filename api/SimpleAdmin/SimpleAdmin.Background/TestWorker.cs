using SimpleAdmin.Plugin.Cache;

namespace SimpleAdmin.Background;

public class TestWorker : BackgroundService
{
    private readonly ILogger<TestWorker> _logger;
    private readonly ISimpleCacheService _simpleCacheService;

    public TestWorker(ILogger<TestWorker> logger, ISimpleCacheService simpleCacheService)
    {
        _logger = logger;
        this._simpleCacheService = simpleCacheService;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {

    }
}
