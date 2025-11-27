using WatchTower.Infrastructure.Mappings;

namespace WatchTower.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddScoped<IDbConnectionFactory>(_ => 
            new MySqlConnectionFactory(configuration.GetConnectionString("DefaultConnection")!));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICelestialBodyRepository, CelestialBodyRepository>();
        services.AddScoped<IDiscoveryRepository, DiscoveryRepository>();
        services.AddScoped<IArticleRepository, ArticleRepository>();
        services.AddScoped<IFavoriteRepository, FavoriteRepository>();
        services.AddScoped<IExplorationRepository, ExplorationRepository>();

        // Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ICelestialBodyService, CelestialBodyService>();
        services.AddScoped<IDiscoveryService, DiscoveryService>();
        services.AddScoped<IArticleService, ArticleService>();
        services.AddScoped<IExportService, ExportService>();

        // External Services
        services.AddHttpClient<INASASyncService, NASAApiClient>();
        
        // Background Services
        services.AddHostedService<NASASyncService>();

        // AutoMapper
        services.AddAutoMapper(typeof(AutoMapperProfile));

        return services;
    }
}