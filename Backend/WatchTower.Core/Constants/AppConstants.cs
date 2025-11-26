namespace WatchTower.Core.Constants;

public static class AppConstants
{
    public const string AppName = "WatchTower";
    public const string AppVersion = "1.0.0";
    
    // JWT Settings
    public const int TokenExpirationHours = 24;
    public const int RefreshTokenExpirationDays = 7;
    
    // Pagination
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;
    
    // Validation
    public const int MinPasswordLength = 8;
    public const int MaxUsernameLength = 50;
    public const int MaxEmailLength = 100;
    
    // Astronomy Constants
    public const double EarthMassInKg = 5.9722e24;
    public const double SolarMassInKg = 1.989e30;
    public const double LightYearInKm = 9.461e12;
    
    // API Limits
    public const int MaxDiscoveryDescriptionLength = 2000;
    public const int MaxArticleContentLength = 10000;
    public const int MaxCommentLength = 1000;
    
    // File Sizes
    public const int MaxImageSizeBytes = 5 * 1024 * 1024; // 5MB
    public const int MaxExportSizeBytes = 50 * 1024 * 1024; // 50MB
}