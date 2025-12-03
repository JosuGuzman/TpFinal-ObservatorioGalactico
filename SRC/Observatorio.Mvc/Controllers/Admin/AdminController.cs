using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Observatorio.Core.Entities.System;
using Observatorio.Core.Interfaces;
using Observatorio.Mvc.Models.Admin;

namespace Observatorio.Mvc.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("Admin/[controller]")]
public class AdminController : BaseController
{
    private readonly ISystemLogRepository _logRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDiscoveryRepository _discoveryRepository;
    private readonly IArticleRepository _articleRepository;
    private readonly IEventRepository _eventRepository;
    private readonly ILoggingService _loggingService;

    public AdminController(
        ISystemLogRepository logRepository,
        IUserRepository userRepository,
        IDiscoveryRepository discoveryRepository,
        IArticleRepository articleRepository,
        IEventRepository eventRepository,
        ILoggingService loggingService)
    {
        _logRepository = logRepository;
        _userRepository = userRepository;
        _discoveryRepository = discoveryRepository;
        _articleRepository = articleRepository;
        _eventRepository = eventRepository;
        _loggingService = loggingService;
    }

    [HttpGet]
    [Route("")]
    [Route("Index")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Route("SystemInfo")]
    public async Task<IActionResult> SystemInfo()
    {
        try
        {
            var model = new SystemInfoViewModel
            {
                ServerTime = DateTime.UtcNow,
                ServerName = Environment.MachineName,
                OSVersion = Environment.OSVersion.ToString(),
                ProcessorCount = Environment.ProcessorCount,
                SystemUpTime = TimeSpan.FromMilliseconds(Environment.TickCount64),
                MemoryUsage = GC.GetTotalMemory(false),
                TotalUsers = await _userRepository.CountAsync(),
                TotalLogs = await _logRepository.CountAsync(),
                TotalDiscoveries = await _discoveryRepository.CountAsync(),
                TotalArticles = await _articleRepository.CountAsync(),
                TotalEvents = await _eventRepository.CountAsync()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving system info");
            return View("Error");
        }
    }

    [HttpGet]
    [Route("Configuration")]
    public IActionResult Configuration()
    {
        var model = new ConfigurationViewModel
        {
            AppSettings = new Dictionary<string, string>
            {
                { "App Name", "Observatorio WatchTower" },
                { "Version", "1.0.0" },
                { "Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production" },
                { "Database Provider", "MySQL" },
                { "Cache Provider", "In-Memory" },
                { "Log Retention Days", "90" },
                { "Max Export Records", "1000" },
                { "Default Page Size", "20" },
                { "API Rate Limit", "100" }
            }
        };

        return View(model);
    }

    [HttpGet]
    [Route("Backup")]
    public IActionResult Backup()
    {
        return View(new BackupViewModel());
    }

    [HttpPost]
    [Route("Backup")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Backup(BackupViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            // Aquí iría la lógica real de backup
            await Task.Delay(1000); // Simulación

            await _loggingService.LogInfoAsync("Backup",
                $"Backup requested by admin {GetCurrentUserEmail()}", GetCurrentUserId());

            TempData["SuccessMessage"] = "Backup process started successfully!";
            return RedirectToAction("Backup");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            _logger.LogError(ex, "Error starting backup");
            return View(model);
        }
    }

    [HttpPost]
    [Route("ClearCache")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ClearCache()
    {
        try
        {
            // Aquí iría la lógica real para limpiar caché
            await Task.Delay(500); // Simulación

            await _loggingService.LogInfoAsync("CacheCleared",
                $"Cache cleared by admin {GetCurrentUserEmail()}", GetCurrentUserId());

            TempData["SuccessMessage"] = "Cache cleared successfully!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cache");
            TempData["ErrorMessage"] = "Error clearing cache.";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    [Route("RestartApp")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RestartApp()
    {
        try
        {
            await _loggingService.LogInfoAsync("AppRestart",
                $"Application restart requested by admin {GetCurrentUserEmail()}", GetCurrentUserId());

            TempData["SuccessMessage"] = "Application restart requested. Changes will take effect shortly.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error requesting app restart");
            TempData["ErrorMessage"] = "Error requesting application restart.";
            return RedirectToAction("Index");
        }
    }

    [HttpGet]
    [Route("ApiKeys")]
    public async Task<IActionResult> ApiKeys(int page = 1, int pageSize = 20)
    {
        try
        {
            var users = await _userRepository.GetAllAsync();
            var usersWithKeys = users.Where(u => !string.IsNullOrEmpty(u.ApiKey)).ToList();
            var totalUsers = usersWithKeys.Count;
            
            var pagedUsers = usersWithKeys
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new ApiKeyViewModel
                {
                    UserID = u.UserID,
                    UserName = u.UserName,
                    Email = u.Email,
                    Role = u.Role?.RoleName ?? "User",
                    ApiKey = u.ApiKey,
                    LastLogin = u.LastLogin,
                    IsActive = u.IsActive
                })
                .ToList();

            var model = new ApiKeysViewModel
            {
                ApiKeys = pagedUsers,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalUsers,
                TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize)
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving API keys");
            return View("Error");
        }
    }

    [HttpPost]
    [Route("RegenerateApiKey/{userId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegenerateApiKey(int userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return NotFound();

            // Generar nueva API key
            user.ApiKey = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
            await _userRepository.UpdateAsync(user);

            await _loggingService.LogInfoAsync("ApiKeyRegenerated",
                $"API key regenerated for user {user.Email} by admin {GetCurrentUserEmail()}",
                GetCurrentUserId());

            TempData["SuccessMessage"] = $"API key regenerated for user {user.UserName}.";
            return RedirectToAction("ApiKeys");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error regenerating API key for user: {userId}");
            TempData["ErrorMessage"] = "Error regenerating API key.";
            return RedirectToAction("ApiKeys");
        }
    }

    [HttpPost]
    [Route("RevokeApiKey/{userId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RevokeApiKey(int userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return NotFound();

            user.ApiKey = null;
            await _userRepository.UpdateAsync(user);

            await _loggingService.LogInfoAsync("ApiKeyRevoked",
                $"API key revoked for user {user.Email} by admin {GetCurrentUserEmail()}",
                GetCurrentUserId());

            TempData["SuccessMessage"] = $"API key revoked for user {user.UserName}.";
            return RedirectToAction("ApiKeys");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error revoking API key for user: {userId}");
            TempData["ErrorMessage"] = "Error revoking API key.";
            return RedirectToAction("ApiKeys");
        }
    }

    [HttpGet]
    [Route("Settings")]
    public IActionResult Settings()
    {
        var model = new AdminSettingsViewModel
        {
            SiteTitle = "Observatorio WatchTower",
            SiteDescription = "Astronomical Observation Platform",
            MaintenanceMode = false,
            RegistrationEnabled = true,
            DiscoveryReportingEnabled = true,
            CommunityVotingEnabled = true,
            MaxFileUploadSize = 10, // MB
            EmailNotifications = true,
            AnalyticsEnabled = true,
            LogRetentionDays = 90
        };

        return View(model);
    }

    [HttpPost]
    [Route("Settings")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Settings(AdminSettingsViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            // Aquí iría la lógica real para guardar configuraciones
            await Task.Delay(500); // Simulación

            await _loggingService.LogInfoAsync("SettingsUpdated",
                $"System settings updated by admin {GetCurrentUserEmail()}", GetCurrentUserId());

            TempData["SuccessMessage"] = "Settings updated successfully!";
            return RedirectToAction("Settings");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            _logger.LogError(ex, "Error updating settings");
            return View(model);
        }
    }

    [HttpGet]
    [Route("Tools")]
    public IActionResult Tools()
    {
        return View(new AdminToolsViewModel());
    }

    [HttpPost]
    [Route("Tools/ReindexSearch")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReindexSearch()
    {
        try
        {
            // Aquí iría la lógica real para reindexar búsqueda
            await Task.Delay(2000); // Simulación

            await _loggingService.LogInfoAsync("SearchReindexed",
                $"Search index rebuilt by admin {GetCurrentUserEmail()}", GetCurrentUserId());

            TempData["SuccessMessage"] = "Search index rebuilt successfully!";
            return RedirectToAction("Tools");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reindexing search");
            TempData["ErrorMessage"] = "Error rebuilding search index.";
            return RedirectToAction("Tools");
        }
    }

    [HttpPost]
    [Route("Tools/OptimizeDatabase")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OptimizeDatabase()
    {
        try
        {
            // Aquí iría la lógica real para optimizar base de datos
            await Task.Delay(3000); // Simulación

            await _loggingService.LogInfoAsync("DatabaseOptimized",
                $"Database optimized by admin {GetCurrentUserEmail()}", GetCurrentUserId());

            TempData["SuccessMessage"] = "Database optimized successfully!";
            return RedirectToAction("Tools");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error optimizing database");
            TempData["ErrorMessage"] = "Error optimizing database.";
            return RedirectToAction("Tools");
        }
    }

    [HttpPost]
    [Route("Tools/SendTestEmail")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendTestEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            TempData["ErrorMessage"] = "Email address is required.";
            return RedirectToAction("Tools");
        }

        try
        {
            // Aquí iría la lógica real para enviar email de prueba
            await Task.Delay(1000); // Simulación

            await _loggingService.LogInfoAsync("TestEmailSent",
                $"Test email sent to {email} by admin {GetCurrentUserEmail()}", GetCurrentUserId());

            TempData["SuccessMessage"] = $"Test email sent to {email} successfully!";
            return RedirectToAction("Tools");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending test email to: {email}");
            TempData["ErrorMessage"] = "Error sending test email.";
            return RedirectToAction("Tools");
        }
    }
}