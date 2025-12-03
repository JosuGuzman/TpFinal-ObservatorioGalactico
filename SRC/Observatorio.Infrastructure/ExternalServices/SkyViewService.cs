namespace Observatorio.Infrastructure.ExternalServices;

public class SkyViewService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public SkyViewService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };

        _httpClient.BaseAddress = new Uri(AppConstants.SKYVIEW_API);
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task<string> GetSkyImageAsync(double ra, double dec, string survey = "DSS2", string scale = "0.25")
    {
        try
        {
            var url = $"current/cgi/runquery.pl?Survey={survey}&Position={ra},{dec}&Scale={scale}";
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"SkyView API error: {ex.Message}", ex);
        }
    }

    public async Task<object> GetAvailableSurveysAsync()
    {
        try
        {
            var url = "current/cgi/survey.pl";
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();
            // Parse HTML to extract survey list
            // This is simplified - actual implementation would parse the HTML
            return new { Surveys = new[] { "DSS2", "2MASS-J", "2MASS-H", "2MASS-K", "WISE" } };
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"SkyView API error: {ex.Message}", ex);
        }
    }
}