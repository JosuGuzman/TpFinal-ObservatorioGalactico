namespace WatchTower.Infrastructure.External;

public class NASAApiClient : INASASyncService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NASAApiClient> _logger;
    private readonly string _apiKey;

    public NASAApiClient(HttpClient httpClient, IConfiguration configuration, ILogger<NASAApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration["NASA:ApiKey"] ?? "DEMO_KEY";
        
        _httpClient.BaseAddress = new Uri("https://api.nasa.gov/");
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task SyncCelestialBodiesAsync()
    {
        try
        {
            _logger.LogInformation("Starting NASA celestial bodies synchronization...");

            // Ejemplo: Obtener Astronomy Picture of the Day
            var apodResponse = await _httpClient.GetAsync($"planetary/apod?api_key={_apiKey}");
            if (apodResponse.IsSuccessStatusCode)
            {
                var apodContent = await apodResponse.Content.ReadAsStringAsync();
                _logger.LogInformation("Successfully retrieved APOD data");
                // Procesar y guardar datos...
            }

            // Aquí se agregarían más llamadas a APIs de NASA según sea necesario

            _logger.LogInformation("NASA celestial bodies synchronization completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during NASA celestial bodies synchronization");
            throw;
        }
    }

    public async Task SyncAstronomicalEventsAsync()
    {
        try
        {
            _logger.LogInformation("Starting NASA astronomical events synchronization...");

            // Implementar lógica para obtener eventos astronómicos
            // Por ejemplo, desde NASA's Event API o otras fuentes

            _logger.LogInformation("NASA astronomical events synchronization completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during NASA astronomical events synchronization");
            throw;
        }
    }

    public Task<DateTime> GetLastSyncDateAsync()
    {
        // En una implementación real, esto vendría de la base de datos
        return Task.FromResult(DateTime.UtcNow.AddDays(-1));
    }

    public async Task<object?> GetPlanetaryDataAsync(string planet)
    {
        try
        {
            var response = await _httpClient.GetAsync($"planetary/earth/imagery?lon=100.75&lat=1.5&date=2014-02-01&api_key={_apiKey}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving planetary data for {Planet}", planet);
            return null;
        }
    }
}