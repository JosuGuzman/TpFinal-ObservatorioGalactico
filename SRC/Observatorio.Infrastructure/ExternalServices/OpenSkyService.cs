namespace Observatorio.Infrastructure.ExternalServices;

public class OpenSkyService
{
    private readonly HttpClient _httpClient;
    private readonly OpenSkyApiSettings _settings;
    private readonly JsonSerializerOptions _jsonOptions;

    public OpenSkyService(HttpClient httpClient, IOptions<ExternalApiSettings> apiSettings)
    {
        _httpClient = httpClient;
        _settings = apiSettings.Value.OpenSky;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };

        _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task<object> GetAllStateVectorsAsync()
    {
        try
        {
            var url = "states/all";
            
            if (!string.IsNullOrEmpty(_settings.Username) && !string.IsNullOrEmpty(_settings.Password))
            {
                var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_settings.Username}:{_settings.Password}"));
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
            }

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(json, _jsonOptions);
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"OpenSky API error: {ex.Message}", ex);
        }
    }

    public async Task<object> GetFlightsByAircraftAsync(string icao24, DateTime? begin = null, DateTime? end = null)
    {
        try
        {
            var url = $"flights/aircraft?icao24={icao24}";
            
            if (begin.HasValue)
                url += $"&begin={(int)(begin.Value - new DateTime(1970, 1, 1)).TotalSeconds}";
            
            if (end.HasValue)
                url += $"&end={(int)(end.Value - new DateTime(1970, 1, 1)).TotalSeconds}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(json, _jsonOptions);
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"OpenSky API error: {ex.Message}", ex);
        }
    }
}