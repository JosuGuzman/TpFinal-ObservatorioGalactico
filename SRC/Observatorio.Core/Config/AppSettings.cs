namespace Observatorio.Core.Config;

public class AppSettings
{
    public string SecretKey { get; set; }
    public int TokenExpirationDays { get; set; } = 7;
    public int MaxExportRecords { get; set; } = 1000;
    public int DefaultPageSize { get; set; } = 20;
    public bool EnableApiRateLimiting { get; set; } = true;
    public int ApiRateLimit { get; set; } = 100;
    
    // APIs externas
    public NasaApiSettings NasaApi { get; set; }
    public ExternalApiSettings OpenSkyNetwork { get; set; }
}

public class NasaApiSettings
{
    public string BaseUrl { get; set; }
    public string ApiKey { get; set; }
}

public class ExternalApiSettings
{
    public string BaseUrl { get; set; }
}