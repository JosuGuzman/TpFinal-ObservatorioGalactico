namespace WatchTower.Infrastructure.BackgroundServices;

public class NASASyncService : BackgroundService
{
    private readonly ILogger<NASASyncService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _syncInterval = TimeSpan.FromHours(24);

    public NASASyncService(ILogger<NASASyncService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("NASA Sync Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var nasaClient = scope.ServiceProvider.GetRequiredService<INASASyncService>();
                
                _logger.LogInformation("Starting NASA data synchronization at: {Time}", DateTimeOffset.Now);
                
                await nasaClient.SyncCelestialBodiesAsync();
                await nasaClient.SyncAstronomicalEventsAsync();
                
                _logger.LogInformation("NASA data synchronization completed at: {Time}", DateTimeOffset.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during NASA data synchronization");
            }

            await Task.Delay(_syncInterval, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("NASA Sync Service is stopping");
        await base.StopAsync(cancellationToken);
    }
}