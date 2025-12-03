namespace Observatorio.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class DashboardController : BaseApiController
{
    private readonly IAstronomicalDataService _astronomicalService;
    private readonly IDiscoveryService _discoveryService;
    private readonly IContentService _contentService;
    private readonly IExportService _exportService;
    private readonly ILoggingService _loggingService;

    public DashboardController(
        IAstronomicalDataService astronomicalService,
        IDiscoveryService discoveryService,
        IContentService contentService,
        IExportService exportService,
        ILoggingService loggingService)
    {
        _astronomicalService = astronomicalService;
        _discoveryService = discoveryService;
        _contentService = contentService;
        _exportService = exportService;
        _loggingService = loggingService;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetDashboardStats()
    {
        try
        {
            var galaxiesCount = await _astronomicalService.GetGalaxiesCountAsync();
            var starsCount = await _astronomicalService.GetStarsCountAsync();
            var planetsCount = await _astronomicalService.GetPlanetsCountAsync();
            var discoveriesCount = await _discoveryService.GetDiscoveriesCountAsync();
            var pendingDiscoveries = await _discoveryService.GetPendingDiscoveriesCountAsync();
            var habitablePlanets = await _astronomicalService.GetHabitablesPlanetsCountAsync();

            var stats = new DashboardStatsResponse
            {
                TotalGalaxies = galaxiesCount,
                TotalStars = starsCount,
                TotalPlanets = planetsCount,
                TotalDiscoveries = discoveriesCount,
                PendingDiscoveries = pendingDiscoveries,
                HabitablePlanets = habitablePlanets
            };

            return SuccessResponse(stats);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving dashboard stats", ex);
        }
    }

    [HttpGet("recent-activity")]
    public async Task<IActionResult> GetRecentActivity([FromQuery] int limit = 10)
    {
        try
        {
            var userId = GetCurrentUserId();
            var userRole = GetCurrentUserRole();

            var activities = new List<object>();

            // Últimos descubrimientos (según rol)
            var discoveries = userRole == "Admin" || userRole == "Astronomer"
                ? await _discoveryService.GetRecentDiscoveriesAsync(limit)
                : await _discoveryService.GetUserDiscoveriesAsync(userId);

            activities.AddRange(discoveries.Take(limit / 2).Select(d => new
            {
                type = "Discovery",
                id = d.DiscoveryID,
                name = d.SuggestedName,
                date = d.CreatedAt,
                status = d.State.ToString()
            }));

            // Próximos eventos
            var upcomingEvents = await _contentService.GetUpcomingEventsAsync(limit / 2);
            activities.AddRange(upcomingEvents.Select(e => new
            {
                type = "Event",
                id = e.EventID,
                name = e.Name,
                date = e.EventDate,
                status = e.IsUpcoming ? "Upcoming" : "Past"
            }));

            // Actividad del usuario (historial de exploración)
            var userActivity = await _loggingService.GetRecentUserActivityAsync(userId, limit / 2);
            activities.AddRange(userActivity.Select(a => new
            {
                type = "UserActivity",
                action = a.EventType,
                description = a.Description,
                date = a.Timestamp
            }));

            return SuccessResponse(activities.OrderByDescending(a => a.date).Take(limit));
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving recent activity", ex);
        }
    }

    [HttpGet("user-stats")]
    public async Task<IActionResult> GetUserStats()
    {
        try
        {
            var userId = GetCurrentUserId();

            var userStats = new
            {
                TotalDiscoveries = await _discoveryService.GetUserDiscoveriesCountAsync(userId),
                ApprovedDiscoveries = await _discoveryService.GetUserApprovedDiscoveriesCountAsync(userId),
                TotalArticles = await _contentService.GetUserArticlesCountAsync(userId),
                PublishedArticles = await _contentService.GetUserPublishedArticlesCountAsync(userId),
                TotalEvents = await _contentService.GetUserEventsCountAsync(userId),
                FavoritesCount = await _contentService.GetUserFavoritesCountAsync(userId)
            };

            return SuccessResponse(userStats);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving user stats", ex);
        }
    }

    [HttpGet("charts")]
    [Authorize(Roles = "Astronomer,Admin")]
    public async Task<IActionResult> GetChartData()
    {
        try
        {
            var chartData = new
            {
                // Datos para gráfico de tipos de galaxias
                galaxyTypes = await GetGalaxyTypeDistribution(),
                
                // Datos para gráfico de estados de descubrimiento
                discoveryStates = await GetDiscoveryStateDistribution(),
                
                // Datos para gráfico de eventos por mes
                eventsByMonth = await GetEventsByMonth(),
                
                // Datos para gráfico de actividad de usuarios
                userActivity = await GetUserActivityLast30Days()
            };

            return SuccessResponse(chartData);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving chart data", ex);
        }
    }

    [HttpGet("admin-stats")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAdminStats()
    {
        try
        {
            var adminStats = new
            {
                TotalUsers = await _loggingService.GetTotalUsersCountAsync(),
                ActiveUsers = await _loggingService.GetActiveUsersCountAsync(),
                NewUsersLast30Days = await _loggingService.GetNewUsersLast30DaysAsync(),
                SystemLogsCount = await _loggingService.GetSystemLogsCountAsync(),
                ErrorLogsCount = await _loggingService.GetErrorLogsCountAsync(),
                ApiUsage = await _loggingService.GetApiUsageStatsAsync()
            };

            return SuccessResponse(adminStats);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving admin stats", ex);
        }
    }

    [HttpGet("quick-links")]
    [Authorize]
    public IActionResult GetQuickLinks()
    {
        try
        {
            var userRole = GetCurrentUserRole();
            var links = new List<object>
            {
                new { name = "Explore Galaxies", url = "/api/v1/galaxies", icon = "galaxy" },
                new { name = "Browse Discoveries", url = "/api/v1/discoveries", icon = "discovery" },
                new { name = "View Events", url = "/api/v1/events", icon = "event" },
                new { name = "Read Articles", url = "/api/v1/articles", icon = "article" }
            };

            if (userRole == "Admin" || userRole == "Astronomer")
            {
                links.Add(new { name = "Pending Discoveries", url = "/api/v1/discoveries/pending", icon = "pending" });
            }

            if (userRole == "Researcher" || userRole == "Astronomer" || userRole == "Admin")
            {
                links.Add(new { name = "Create Article", url = "/api/v1/articles/create", icon = "write" });
                links.Add(new { name = "Report Discovery", url = "/api/v1/discoveries/report", icon = "report" });
            }

            if (userRole == "Admin")
            {
                links.Add(new { name = "User Management", url = "/api/v1/users", icon = "users" });
                links.Add(new { name = "System Logs", url = "/api/v1/admin/logs", icon = "logs" });
            }

            return SuccessResponse(links);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving quick links", ex);
        }
    }

    // Métodos auxiliares
    private async Task<object> GetGalaxyTypeDistribution()
    {
        var galaxies = await _astronomicalService.GetAllGalaxiesAsync();
        return galaxies
            .GroupBy(g => g.Type)
            .Select(g => new { type = g.Key.ToString(), count = g.Count() })
            .ToList();
    }

    private async Task<object> GetDiscoveryStateDistribution()
    {
        var discoveries = await _discoveryService.GetAllDiscoveriesAsync();
        return discoveries
            .GroupBy(d => d.State)
            .Select(g => new { state = g.Key.ToString(), count = g.Count() })
            .ToList();
    }

    private async Task<object> GetEventsByMonth()
    {
        var events = await _contentService.GetAllEventsAsync();
        var now = DateTime.UtcNow;
        
        return events
            .Where(e => e.EventDate.Year == now.Year)
            .GroupBy(e => e.EventDate.Month)
            .Select(g => new { month = g.Key, count = g.Count() })
            .OrderBy(g => g.month)
            .ToList();
    }

    private async Task<object> GetUserActivityLast30Days()
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-30);
        var activity = await _loggingService.GetActivitySinceAsync(cutoffDate);
        
        return activity
            .GroupBy(a => a.Timestamp.Date)
            .Select(g => new { date = g.Key.ToString("yyyy-MM-dd"), count = g.Count() })
            .OrderBy(g => g.date)
            .ToList();
    }
}

// Extensiones para servicios
public static class ServiceExtensions
{
    public static Task<int> GetStarsCountAsync(this IAstronomicalDataService service) => Task.FromResult(0);
    public static Task<int> GetPlanetsCountAsync(this IAstronomicalDataService service) => Task.FromResult(0);
    public static Task<int> GetHabitablesPlanetsCountAsync(this IAstronomicalDataService service) => Task.FromResult(0);
    public static Task<int> GetDiscoveriesCountAsync(this IDiscoveryService service) => Task.FromResult(0);
    public static Task<int> GetPendingDiscoveriesCountAsync(this IDiscoveryService service) => Task.FromResult(0);
    public static Task<IEnumerable<Discovery>> GetRecentDiscoveriesAsync(this IDiscoveryService service, int limit) 
        => Task.FromResult(Enumerable.Empty<Discovery>());
    public static Task<int> GetUserDiscoveriesCountAsync(this IDiscoveryService service, int userId) => Task.FromResult(0);
    public static Task<int> GetUserApprovedDiscoveriesCountAsync(this IDiscoveryService service, int userId) => Task.FromResult(0);
    public static Task<int> GetUserArticlesCountAsync(this IContentService service, int userId) => Task.FromResult(0);
    public static Task<int> GetUserPublishedArticlesCountAsync(this IContentService service, int userId) => Task.FromResult(0);
    public static Task<int> GetUserEventsCountAsync(this IContentService service, int userId) => Task.FromResult(0);
    public static Task<int> GetUserFavoritesCountAsync(this IContentService service, int userId) => Task.FromResult(0);
    
    // Extensiones para ILoggingService
    public static Task<int> GetTotalUsersCountAsync(this ILoggingService service) => Task.FromResult(0);
    public static Task<int> GetActiveUsersCountAsync(this ILoggingService service) => Task.FromResult(0);
    public static Task<int> GetNewUsersLast30DaysAsync(this ILoggingService service) => Task.FromResult(0);
    public static Task<int> GetSystemLogsCountAsync(this ILoggingService service) => Task.FromResult(0);
    public static Task<int> GetErrorLogsCountAsync(this ILoggingService service) => Task.FromResult(0);
    public static Task<object> GetApiUsageStatsAsync(this ILoggingService service) => Task.FromResult<object>(null);
    public static Task<IEnumerable<SystemLog>> GetRecentUserActivityAsync(this ILoggingService service, int userId, int limit) 
        => Task.FromResult(Enumerable.Empty<SystemLog>());
    public static Task<IEnumerable<SystemLog>> GetActivitySinceAsync(this ILoggingService service, DateTime since) 
        => Task.FromResult(Enumerable.Empty<SystemLog>());
}