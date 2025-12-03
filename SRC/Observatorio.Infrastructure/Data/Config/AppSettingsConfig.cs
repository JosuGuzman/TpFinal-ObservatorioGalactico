namespace Observatorio.Infrastructure.Data.Config;

public class AppSettingsConfig
{
    public JwtSettings Jwt { get; set; }
    public ExternalApiSettings ExternalApis { get; set; }
}

public class JwtSettings
{
    public string SecretKey { get; set; }
    public int TokenExpirationDays { get; set; } = 7;
    public string Issuer { get; set; }
    public string Audience { get; set; }
}

public class ExternalApiSettings
{
    public NasaApiSettings Nasa { get; set; }
    public OpenSkyApiSettings OpenSky { get; set; }
}

public class NasaApiSettings
{
    public string BaseUrl { get; set; }
    public string ApiKey { get; set; }
    public int TimeoutSeconds { get; set; } = 30;
}

public class OpenSkyApiSettings
{
    public string BaseUrl { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}