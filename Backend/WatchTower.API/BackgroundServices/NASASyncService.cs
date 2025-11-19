using WatchTower.API.Repositories;

namespace WatchTower.API.BackgroundServices;

public class NASASyncService : BackgroundService
{
    private readonly ILogger<NASASyncService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly HttpClient _httpClient;

    public NASASyncService(ILogger<NASASyncService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _httpClient = new HttpClient();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("NASA Sync Service running at: {time}", DateTimeOffset.Now);
                
                using var scope = _serviceProvider.CreateScope();
                // Aquí implementarías la lógica de sincronización con NASA APIs
                
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en NASA Sync Service");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}