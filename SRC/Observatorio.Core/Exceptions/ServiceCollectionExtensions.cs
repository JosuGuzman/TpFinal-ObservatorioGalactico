namespace Observatorio.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // Servicios
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAstronomicalDataService, AstronomicalDataService>();
        services.AddScoped<IDiscoveryService, DiscoveryService>();
        services.AddScoped<IContentService, ContentService>();
        services.AddScoped<IExportService, ExportService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<ILoggingService, LoggingService>();
        
        // Authentication Service se configura con parámetros específicos
        services.AddScoped<IAuthenticationService>(provider =>
        {
            // En producción, esto vendría de configuración
            var secretKey = "your-super-secret-key-for-jwt-change-in-production";
            var tokenExpirationDays = 7;
            return new AuthenticationService(secretKey, tokenExpirationDays);
        });

        // Helpers
        services.AddSingleton<PasswordHasher>();

        return services;
    }
}