namespace Observatorio.Infrastructure.ExternalServices;

public class NasaApiService
{
    private readonly HttpClient _httpClient;
    private readonly NasaApiSettings _settings;
    private readonly JsonSerializerOptions _jsonOptions;

    public NasaApiService(HttpClient httpClient, IOptions<ExternalApiSettings> apiSettings)
    {
        _httpClient = httpClient;
        _settings = apiSettings.Value.Nasa;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };

        _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
    }

    public async Task<object> GetAstronomyPictureOfTheDayAsync(DateTime? date = null)
    {
        try
        {
            var url = date.HasValue 
                ? $"planetary/apod?api_key={_settings.ApiKey}&date={date.Value:yyyy-MM-dd}" 
                : $"planetary/apod?api_key={_settings.ApiKey}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(json, _jsonOptions);
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"NASA API error: {ex.Message}", ex);
        }
    }

    public async Task<object> GetMarsRoverPhotosAsync(string rover = "curiosity", DateTime? earthDate = null, int? sol = null)
    {
        try
        {
            var url = $"mars-photos/api/v1/rovers/{rover}/photos?api_key={_settings.ApiKey}";
            
            if (earthDate.HasValue)
                url += $"&earth_date={earthDate.Value:yyyy-MM-dd}";
            else if (sol.HasValue)
                url += $"&sol={sol.Value}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(json, _jsonOptions);
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"NASA API error: {ex.Message}", ex);
        }
    }

    public async Task<object> GetNearEarthObjectsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var start = startDate ?? DateTime.Today;
            var end = endDate ?? startDate ?? DateTime.Today.AddDays(7);
            
            var url = $"neo/rest/v1/feed?start_date={start:yyyy-MM-dd}&end_date={end:yyyy-MM-dd}&api_key={_settings.ApiKey}";
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(json, _jsonOptions);
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"NASA API error: {ex.Message}", ex);
        }
    }
}