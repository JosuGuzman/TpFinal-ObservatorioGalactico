using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Observatorio.Core.Entities.Content;
using Observatorio.Core.Interfaces;
using Observatorio.Mvc.Models.Admin;

namespace Observatorio.Mvc.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("Admin/[controller]")]
public class DashboardController : BaseController
{
    private readonly IAstronomicalDataService _astronomicalService;
    private readonly IDiscoveryService _discoveryService;
    private readonly IContentService _contentService;
    private readonly IUserService _userService;
    private readonly ILoggingService _loggingService;

    public DashboardController(
        IAstronomicalDataService astronomicalService,
        IDiscoveryService discoveryService,
        IContentService contentService,
        IUserService userService,
        ILoggingService loggingService)
    {
        _astronomicalService = astronomicalService;
        _discoveryService = discoveryService;
        _contentService = contentService;
        _userService = userService;
        _loggingService = loggingService;
    }

    [HttpGet]
    [Route("")]
    [Route("Index")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var galaxiesCount = await _astronomicalService.GetGalaxiesCountAsync();
            var starsCount = await _astronomicalService.GetStarsCountAsync();
            var planetsCount = await _astronomicalService.GetPlanetsCountAsync();
            var discoveriesCount = await _discoveryService.GetDiscoveriesCountAsync();
            var pendingDiscoveries = await _discoveryService.GetPendingDiscoveriesCountAsync();
            var habitablePlanets = await _astronomicalService.GetHabitablePlanetsCountAsync();
            var totalUsers = await _userService.GetTotalUsersCountAsync();
            var activeUsers = await _userService.GetActiveUsersCountAsync();

            var model = new DashboardViewModel
            {
                TotalGalaxies = galaxiesCount,
                TotalStars = starsCount,
                TotalPlanets = planetsCount,
                TotalUsers = totalUsers,
                TotalDiscoveries = discoveriesCount,
                TotalArticles = await _contentService.GetTotalArticlesCountAsync(),
                TotalEvents = await _contentService.GetTotalEventsCountAsync(),
                PendingDiscoveries = pendingDiscoveries,
                ActiveUsers = activeUsers,
                HabitablePlanets = habitablePlanets,
                NewUsersToday = await _userService.GetNewUsersTodayCountAsync(),
                NewDiscoveriesToday = await _discoveryService.GetNewDiscoveriesTodayCountAsync()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dashboard data");
            return View("Error");
        }
    }

    [HttpGet]
    [Route("Activity")]
    public async Task<IActionResult> Activity(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            startDate ??= DateTime.UtcNow.AddDays(-30);
            endDate ??= DateTime.UtcNow;

            var logs = await _loggingService.GetLogsAsync(startDate, endDate);
            
            var model = new ActivityViewModel
            {
                StartDate = startDate.Value,
                EndDate = endDate.Value,
                ActivityLogs = logs.Select(l => new ActivityLogViewModel
                {
                    LogID = l.LogID,
                    EventType = l.EventType,
                    Description = l.Description,
                    UserName = l.User?.DisplayName ?? "System",
                    Timestamp = l.Timestamp,
                    IPAddress = l.IPAddress,
                    Status = l.Status,
                    IsSuccess = l.IsSuccess,
                    IsError = l.IsError
                }).ToList(),
                TotalEvents = logs.Count(),
                SuccessEvents = logs.Count(l => l.IsSuccess),
                ErrorEvents = logs.Count(l => l.IsError),
                WarningEvents = logs.Count(l => l.Status?.ToLower() == "warning"),
                SecurityEvents = logs.Count(l => l.Status?.ToLower() == "security")
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving activity logs");
            return View("Error");
        }
    }

    [HttpGet]
    [Route("Charts")]
    public async Task<IActionResult> Charts(string period = "month")
    {
        try
        {
            var model = new ChartsViewModel
            {
                Period = period,
                UserRegistrations = await GetUserRegistrationsData(period),
                DiscoveryReports = await GetDiscoveryReportsData(period),
                ArticlePublications = await GetArticlePublicationsData(period),
                EventCreations = await GetEventCreationsData(period),
                UserActivity = await GetUserActivityData(period),
                SystemUsage = await GetSystemUsageData(period)
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving charts data");
            return View("Error");
        }
    }

    [HttpGet]
    [Route("Reports")]
    public IActionResult Reports()
    {
        var model = new ReportsViewModel
        {
            AvailableReports = new List<ReportTypeViewModel>
            {
                new ReportTypeViewModel { Id = "users", Name = "User Report", Description = "Detailed user statistics and activity" },
                new ReportTypeViewModel { Id = "discoveries", Name = "Discoveries Report", Description = "Discovery submission and validation statistics" },
                new ReportTypeViewModel { Id = "content", Name = "Content Report", Description = "Articles and events publication statistics" },
                new ReportTypeViewModel { Id = "system", Name = "System Report", Description = "System performance and usage statistics" },
                new ReportTypeViewModel { Id = "astronomical", Name = "Astronomical Data Report", Description = "Galaxies, stars, and planets statistics" }
            }
        };

        return View(model);
    }

    [HttpPost]
    [Route("GenerateReport")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateReport(string reportType, DateTime? startDate, DateTime? endDate, string format = "pdf")
    {
        try
        {
            // Aquí iría la lógica real para generar reportes
            await Task.Delay(2000); // Simulación

            var reportContent = await GenerateReportContent(reportType, startDate, endDate);
            
            await _loggingService.LogInfoAsync("ReportGenerated",
                $"{reportType} report generated by admin {GetCurrentUserEmail()}", GetCurrentUserId());

            TempData["SuccessMessage"] = $"{reportType} report generated successfully!";
            
            if (format == "csv")
            {
                return File(System.Text.Encoding.UTF8.GetBytes(reportContent), "text/csv", $"{reportType}-report-{DateTime.UtcNow:yyyyMMdd}.csv");
            }
            else if (format == "json")
            {
                return File(System.Text.Encoding.UTF8.GetBytes(reportContent), "application/json", $"{reportType}-report-{DateTime.UtcNow:yyyyMMdd}.json");
            }
            else
            {
                // Para PDF necesitarías una librería como iTextSharp
                TempData["ReportContent"] = reportContent;
                return RedirectToAction("ViewReport", new { reportType });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error generating {reportType} report");
            TempData["ErrorMessage"] = $"Error generating {reportType} report.";
            return RedirectToAction("Reports");
        }
    }

    [HttpGet]
    [Route("ViewReport/{reportType}")]
    public IActionResult ViewReport(string reportType)
    {
        if (TempData["ReportContent"] is not string reportContent)
            return RedirectToAction("Reports");

        var model = new ReportViewModel
        {
            ReportType = reportType,
            GeneratedAt = DateTime.UtcNow,
            Content = reportContent
        };

        return View(model);
    }

    [HttpGet]
    [Route("QuickStats")]
    public async Task<IActionResult> QuickStats()
    {
        try
        {
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);
            var lastWeek = today.AddDays(-7);
            var lastMonth = today.AddDays(-30);

            var model = new QuickStatsViewModel
            {
                Today = new DailyStatsViewModel
                {
                    NewUsers = await _userService.GetNewUsersCountAsync(today, today.AddDays(1)),
                    NewDiscoveries = await _discoveryService.GetNewDiscoveriesCountAsync(today, today.AddDays(1)),
                    NewArticles = await _contentService.GetNewArticlesCountAsync(today, today.AddDays(1)),
                    NewEvents = await _contentService.GetNewEventsCountAsync(today, today.AddDays(1))
                },
                Yesterday = new DailyStatsViewModel
                {
                    NewUsers = await _userService.GetNewUsersCountAsync(yesterday, today),
                    NewDiscoveries = await _discoveryService.GetNewDiscoveriesCountAsync(yesterday, today),
                    NewArticles = await _contentService.GetNewArticlesCountAsync(yesterday, today),
                    NewEvents = await _contentService.GetNewEventsCountAsync(yesterday, today)
                },
                Last7Days = new DailyStatsViewModel
                {
                    NewUsers = await _userService.GetNewUsersCountAsync(lastWeek, today),
                    NewDiscoveries = await _discoveryService.GetNewDiscoveriesCountAsync(lastWeek, today),
                    NewArticles = await _contentService.GetNewArticlesCountAsync(lastWeek, today),
                    NewEvents = await _contentService.GetNewEventsCountAsync(lastWeek, today)
                },
                Last30Days = new DailyStatsViewModel
                {
                    NewUsers = await _userService.GetNewUsersCountAsync(lastMonth, today),
                    NewDiscoveries = await _discoveryService.GetNewDiscoveriesCountAsync(lastMonth, today),
                    NewArticles = await _contentService.GetNewArticlesCountAsync(lastMonth, today),
                    NewEvents = await _contentService.GetNewEventsCountAsync(lastMonth, today)
                }
            };

            return PartialView("_QuickStats", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving quick stats");
            return Content("Error loading stats");
        }
    }

    private async Task<List<ChartDataViewModel>> GetUserRegistrationsData(string period)
    {
        // Datos de ejemplo para gráficos
        return new List<ChartDataViewModel>
        {
            new ChartDataViewModel { Label = "Jan", Value = 45 },
            new ChartDataViewModel { Label = "Feb", Value = 52 },
            new ChartDataViewModel { Label = "Mar", Value = 48 },
            new ChartDataViewModel { Label = "Apr", Value = 61 },
            new ChartDataViewModel { Label = "May", Value = 55 },
            new ChartDataViewModel { Label = "Jun", Value = 67 }
        };
    }

    private async Task<List<ChartDataViewModel>> GetDiscoveryReportsData(string period)
    {
        return new List<ChartDataViewModel>
        {
            new ChartDataViewModel { Label = "Jan", Value = 23 },
            new ChartDataViewModel { Label = "Feb", Value = 31 },
            new ChartDataViewModel { Label = "Mar", Value = 28 },
            new ChartDataViewModel { Label = "Apr", Value = 42 },
            new ChartDataViewModel { Label = "May", Value = 37 },
            new ChartDataViewModel { Label = "Jun", Value = 49 }
        };
    }

    private async Task<List<ChartDataViewModel>> GetArticlePublicationsData(string period)
    {
        return new List<ChartDataViewModel>
        {
            new ChartDataViewModel { Label = "Jan", Value = 12 },
            new ChartDataViewModel { Label = "Feb", Value = 18 },
            new ChartDataViewModel { Label = "Mar", Value = 15 },
            new ChartDataViewModel { Label = "Apr", Value = 22 },
            new ChartDataViewModel { Label = "May", Value = 19 },
            new ChartDataViewModel { Label = "Jun", Value = 25 }
        };
    }

    private async Task<List<ChartDataViewModel>> GetEventCreationsData(string period)
    {
        return new List<ChartDataViewModel>
        {
            new ChartDataViewModel { Label = "Jan", Value = 8 },
            new ChartDataViewModel { Label = "Feb", Value = 11 },
            new ChartDataViewModel { Label = "Mar", Value = 9 },
            new ChartDataViewModel { Label = "Apr", Value = 14 },
            new ChartDataViewModel { Label = "May", Value = 12 },
            new ChartDataViewModel { Label = "Jun", Value = 16 }
        };
    }

    private async Task<List<ChartDataViewModel>> GetUserActivityData(string period)
    {
        return new List<ChartDataViewModel>
        {
            new ChartDataViewModel { Label = "Mon", Value = 1250 },
            new ChartDataViewModel { Label = "Tue", Value = 1420 },
            new ChartDataViewModel { Label = "Wed", Value = 1380 },
            new ChartDataViewModel { Label = "Thu", Value = 1560 },
            new ChartDataViewModel { Label = "Fri", Value = 1480 },
            new ChartDataViewModel { Label = "Sat", Value = 1620 },
            new ChartDataViewModel { Label = "Sun", Value = 1520 }
        };
    }

    private async Task<List<ChartDataViewModel>> GetSystemUsageData(string period)
    {
        return new List<ChartDataViewModel>
        {
            new ChartDataViewModel { Label = "CPU", Value = 65 },
            new ChartDataViewModel { Label = "Memory", Value = 78 },
            new ChartDataViewModel { Label = "Disk", Value = 42 },
            new ChartDataViewModel { Label = "Network", Value = 55 }
        };
    }

    private async Task<string> GenerateReportContent(string reportType, DateTime? startDate, DateTime? endDate)
    {
        var report = new System.Text.StringBuilder();
        report.AppendLine($"Report Type: {reportType}");
        report.AppendLine($"Period: {startDate?.ToString("yyyy-MM-dd") ?? "All time"} to {endDate?.ToString("yyyy-MM-dd") ?? "Present"}");
        report.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
        report.AppendLine("=".PadRight(50, '='));

        switch (reportType.ToLower())
        {
            case "users":
                var totalUsers = await _userService.GetTotalUsersCountAsync();
                var activeUsers = await _userService.GetActiveUsersCountAsync();
                report.AppendLine($"Total Users: {totalUsers}");
                report.AppendLine($"Active Users: {activeUsers}");
                report.AppendLine($"Inactive Users: {totalUsers - activeUsers}");
                break;

            case "discoveries":
                var totalDiscoveries = await _discoveryService.GetDiscoveriesCountAsync();
                var pendingDiscoveries = await _discoveryService.GetPendingDiscoveriesCountAsync();
                report.AppendLine($"Total Discoveries: {totalDiscoveries}");
                report.AppendLine($"Pending Discoveries: {pendingDiscoveries}");
                report.AppendLine($"Approved Discoveries: {await _discoveryService.GetApprovedDiscoveriesCountAsync()}");
                break;

            case "content":
                report.AppendLine($"Total Articles: {await _contentService.GetTotalArticlesCountAsync()}");
                report.AppendLine($"Published Articles: {await _contentService.GetPublishedArticlesCountAsync()}");
                report.AppendLine($"Total Events: {await _contentService.GetTotalEventsCountAsync()}");
                report.AppendLine($"Upcoming Events: {await _contentService.GetUpcomingEventsCountAsync()}");
                break;

            case "system":
                report.AppendLine("System Performance Report");
                report.AppendLine($"Server Time: {DateTime.UtcNow}");
                report.AppendLine($"Uptime: {TimeSpan.FromMilliseconds(Environment.TickCount64)}");
                report.AppendLine($"Memory Usage: {GC.GetTotalMemory(false) / 1024 / 1024} MB");
                break;

            case "astronomical":
                report.AppendLine($"Total Galaxies: {await _astronomicalService.GetGalaxiesCountAsync()}");
                report.AppendLine($"Total Stars: {await _astronomicalService.GetStarsCountAsync()}");
                report.AppendLine($"Total Planets: {await _astronomicalService.GetPlanetsCountAsync()}");
                report.AppendLine($"Habitable Planets: {await _astronomicalService.GetHabitablePlanetsCountAsync()}");
                break;
        }

        return report.ToString();
    }
}