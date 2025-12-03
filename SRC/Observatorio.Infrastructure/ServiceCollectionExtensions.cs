namespace Observatorio.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configuración
        services.Configure<DatabaseConfig>(configuration.GetSection("Database"));
        services.Configure<AppSettingsConfig>(configuration.GetSection("AppSettings"));

        // Contextos de base de datos
        services.AddDbContext<DatabaseContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            
            if (configuration.GetValue<bool>("Database:EnableDetailedErrors"))
                options.EnableDetailedErrors();
            
            if (configuration.GetValue<bool>("Database:EnableSensitiveDataLogging"))
                options.EnableSensitiveDataLogging();
        });

        services.AddSingleton<DapperContext>();

        // Repositorios
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IGalaxyRepository, GalaxyRepository>();
        services.AddScoped<IStarRepository, StarRepository>();
        services.AddScoped<IPlanetRepository, PlanetRepository>();
        services.AddScoped<IDiscoveryRepository, DiscoveryRepository>();
        services.AddScoped<IDiscoveryVoteRepository, DiscoveryVoteRepository>();
        services.AddScoped<IArticleRepository, ArticleRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<ISystemLogRepository, SystemLogRepository>();
        services.AddScoped<IUserFavoriteRepository, UserFavoriteRepository>();
        services.AddScoped<IExplorationHistoryRepository, ExplorationHistoryRepository>();
        services.AddScoped<ISavedSearchRepository, SavedSearchRepository>();

        // Servicios externos
        services.AddHttpClient<NasaApiService>();
        services.AddHttpClient<OpenSkyService>();
        services.AddHttpClient<SkyViewService>();

        // Servicios de aplicación
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IAstronomicalDataService, AstronomicalDataService>();
        services.AddScoped<IDiscoveryService, DiscoveryService>();
        services.AddScoped<IContentService, ContentService>();
        services.AddScoped<IExportService, ExportService>();
        services.AddScoped<ILoggingService, LoggingService>();
        services.AddScoped<IUserProfileService, UserProfileService>();

        return services;
    }
}