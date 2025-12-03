using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Observatorio.Core.Entities.System;
using Observatorio.Core.Interfaces;
using Observatorio.Mvc.Models.Admin;

namespace Observatorio.Mvc.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("Admin/[controller]")]
public class SystemLogController : BaseController
{
    private readonly ISystemLogRepository _logRepository;
    private readonly ILoggingService _loggingService;

    public SystemLogController(
        ISystemLogRepository logRepository,
        ILoggingService loggingService)
    {
        _logRepository = logRepository;
        _loggingService = loggingService;
    }

    [HttpGet]
    [Route("")]
    [Route("Index")]
    public async Task<IActionResult> Index(
        string eventType = "",
        string status = "",
        int? userId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string search = "",
        int page = 1,
        int pageSize = 50,
        string sortBy = "Timestamp",
        bool sortDesc = true)
    {
        try
        {
            startDate ??= DateTime.UtcNow.AddDays(-7);
            endDate ??= DateTime.UtcNow.AddDays(1);

            var logs = await _logRepository.GetAllAsync();
            
            // Aplicar filtros de fecha
            logs = logs.Where(l => l.Timestamp >= startDate && l.Timestamp <= endDate).ToList();

            // Aplicar otros filtros
            if (!string.IsNullOrEmpty(eventType))
                logs = logs.Where(l => l.EventType.Contains(eventType, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrEmpty(status))
                logs = logs.Where(l => l.Status?.Equals(status, StringComparison.OrdinalIgnoreCase) ?? false).ToList();

            if (userId.HasValue)
                logs = logs.Where(l => l.UserID == userId.Value).ToList();

            if (!string.IsNullOrEmpty(search))
            {
                logs = logs.Where(l => 
                    l.EventType.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    l.Description.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (l.User?.DisplayName?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (l.IPAddress?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)
                ).ToList();
            }

            // Aplicar ordenamiento
            logs = sortBy.ToLower() switch
            {
                "eventtype" => sortDesc ? logs.OrderByDescending(l => l.EventType).ToList() : logs.OrderBy(l => l.EventType).ToList(),
                "user" => sortDesc ? logs.OrderByDescending(l => l.User?.DisplayName).ToList() : logs.OrderBy(l => l.User?.DisplayName).ToList(),
                "ipaddress" => sortDesc ? logs.OrderByDescending(l => l.IPAddress).ToList() : logs.OrderBy(l => l.IPAddress).ToList(),
                "status" => sortDesc ? logs.OrderByDescending(l => l.Status).ToList() : logs.OrderBy(l => l.Status).ToList(),
                _ => sortDesc ? logs.OrderByDescending(l => l.Timestamp).ToList() : logs.OrderBy(l => l.Timestamp).ToList()
            };

            var totalLogs = logs.Count;
            var pagedLogs = logs
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new SystemLogViewModel
                {
                    LogID = l.LogID,
                    EventType = l.EventType,
                    Description = l.Description,
                    UserID = l.UserID,
                    UserName = l.User?.DisplayName ?? "System",
                    Timestamp = l.Timestamp,
                    IPAddress = l.IPAddress,
                    Status = l.Status,
                    IsSuccess = l.IsSuccess,
                    IsError = l.IsError
                })
                .ToList();

            var model = new SystemLogsViewModel
            {
                Logs = pagedLogs,
                EventType = eventType,
                Status = status,
                UserId = userId,
                StartDate = startDate.Value,
                EndDate = endDate.Value,
                Search = search,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalLogs,
                TotalPages = (int)Math.Ceiling(totalLogs / (double)pageSize),
                SortBy = sortBy,
                SortDescending = sortDesc,
                AvailableEventTypes = await GetAvailableEventTypes(),
                AvailableStatuses = new List<string> { "", "INFO", "WARNING", "ERROR", "SECURITY" },
                Stats = new LogStatsViewModel
                {
                    TotalLogs = totalLogs,
                    InfoLogs = logs.Count(l => l.Status?.ToUpper() == "INFO"),
                    WarningLogs = logs.Count(l => l.Status?.ToUpper() == "WARNING"),
                    ErrorLogs = logs.Count(l => l.Status?.ToUpper() == "ERROR"),
                    SecurityLogs = logs.Count(l => l.Status?.ToUpper() == "SECURITY"),
                    UniqueUsers = logs.Where(l => l.UserID.HasValue).Select(l => l.UserID.Value).Distinct().Count(),
                    UniqueIPs = logs.Where(l => !string.IsNullOrEmpty(l.IPAddress)).Select(l => l.IPAddress).Distinct().Count()
                }
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving system logs");
            return View("Error");
        }
    }

    [HttpGet]
    [Route("Details/{id}")]
    public async Task<IActionResult> Details(long id)
    {
        try
        {
            var log = await _logRepository.GetByIdAsync((int)id);
            if (log == null)
                return NotFound();

            var model = new SystemLogDetailViewModel
            {
                LogID = log.LogID,
                EventType = log.EventType,
                Description = log.Description,
                UserID = log.UserID,
                UserName = log.User?.DisplayName ?? "System",
                Timestamp = log.Timestamp,
                IPAddress = log.IPAddress,
                Status = log.Status,
                IsSuccess = log.IsSuccess,
                IsError = log.IsError,
                RawData = FormatLogDescription(log.Description)
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving log details: {id}");
            return View("Error");
        }
    }

    [HttpGet]
    [Route("Live")]
    public IActionResult Live()
    {
        return View(new LiveLogsViewModel());
    }

    [HttpGet]
    [Route("GetLiveLogs")]
    public async Task<IActionResult> GetLiveLogs(int lastLogId = 0)
    {
        try
        {
            var logs = await _logRepository.GetAllAsync();
            var newLogs = logs
                .Where(l => l.LogID > lastLogId)
                .OrderByDescending(l => l.Timestamp)
                .Take(20)
                .Select(l => new SystemLogViewModel
                {
                    LogID = l.LogID,
                    EventType = l.EventType,
                    Description = l.Description.Length > 100 ? l.Description.Substring(0, 100) + "..." : l.Description,
                    UserName = l.User?.DisplayName ?? "System",
                    Timestamp = l.Timestamp,
                    Status = l.Status,
                    IsSuccess = l.IsSuccess,
                    IsError = l.IsError
                })
                .ToList();

            return Json(new
            {
                success = true,
                logs = newLogs,
                lastLogId = newLogs.Any() ? newLogs.Max(l => l.LogID) : lastLogId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving live logs");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    [Route("Statistics")]
    public async Task<IActionResult> Statistics(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            startDate ??= DateTime.UtcNow.AddDays(-30);
            endDate ??= DateTime.UtcNow;

            var logs = await _logRepository.GetAllAsync();
            logs = logs.Where(l => l.Timestamp >= startDate && l.Timestamp <= endDate).ToList();

            var model = new LogStatisticsViewModel
            {
                StartDate = startDate.Value,
                EndDate = endDate.Value,
                TotalLogs = logs.Count,
                LogsByType = logs
                    .GroupBy(l => l.EventType)
                    .Select(g => new LogTypeStatViewModel
                    {
                        EventType = g.Key,
                        Count = g.Count(),
                        Percentage = (double)g.Count() / logs.Count * 100
                    })
                    .OrderByDescending(s => s.Count)
                    .ToList(),
                LogsByStatus = logs
                    .GroupBy(l => l.Status ?? "UNKNOWN")
                    .Select(g => new LogStatusStatViewModel
                    {
                        Status = g.Key,
                        Count = g.Count(),
                        Percentage = (double)g.Count() / logs.Count * 100
                    })
                    .OrderByDescending(s => s.Count)
                    .ToList(),
                LogsByUser = logs
                    .Where(l => l.UserID.HasValue)
                    .GroupBy(l => l.UserID.Value)
                    .Select(g => new LogUserStatViewModel
                    {
                        UserID = g.Key,
                        UserName = g.First().User?.DisplayName ?? $"User {g.Key}",
                        Count = g.Count(),
                        LastActivity = g.Max(l => l.Timestamp)
                    })
                    .OrderByDescending(s => s.Count)
                    .Take(10)
                    .ToList(),
                LogsByHour = GetLogsByHour(logs),
                LogsByDay = GetLogsByDay(logs),
                ErrorTrend = GetErrorTrend(logs),
                BusiestHours = GetBusiestHours(logs)
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving log statistics");
            return View("Error");
        }
    }

    [HttpGet]
    [Route("Errors")]
    public async Task<IActionResult> Errors(int page = 1, int pageSize = 20)
    {
        try
        {
            var logs = await _logRepository.GetAllAsync();
            var errorLogs = logs
                .Where(l => l.IsError)
                .OrderByDescending(l => l.Timestamp)
                .ToList();

            var totalLogs = errorLogs.Count;
            var pagedLogs = errorLogs
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new ErrorLogViewModel
                {
                    LogID = l.LogID,
                    EventType = l.EventType,
                    Description = l.Description.Length > 200 ? l.Description.Substring(0, 200) + "..." : l.Description,
                    UserName = l.User?.DisplayName ?? "System",
                    Timestamp = l.Timestamp,
                    IPAddress = l.IPAddress
                })
                .ToList();

            var model = new ErrorLogsViewModel
            {
                ErrorLogs = pagedLogs,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalLogs,
                TotalPages = (int)Math.Ceiling(totalLogs / (double)pageSize),
                TotalErrors = totalLogs,
                UniqueErrors = errorLogs.Select(l => l.EventType).Distinct().Count(),
                Last24Hours = errorLogs.Count(l => l.Timestamp > DateTime.UtcNow.AddHours(-24)),
                MostCommonError = errorLogs
                    .GroupBy(l => l.EventType)
                    .Select(g => new { Type = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .FirstOrDefault()?.Type ?? "None"
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving error logs");
            return View("Error");
        }
    }

    [HttpGet]
    [Route("Security")]
    public async Task<IActionResult> Security(int page = 1, int pageSize = 20)
    {
        try
        {
            var logs = await _logRepository.GetAllAsync();
            var securityLogs = logs
                .Where(l => l.Status?.ToUpper() == "SECURITY")
                .OrderByDescending(l => l.Timestamp)
                .ToList();

            var totalLogs = securityLogs.Count;
            var pagedLogs = securityLogs
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new SecurityLogViewModel
                {
                    LogID = l.LogID,
                    EventType = l.EventType,
                    Description = l.Description.Length > 150 ? l.Description.Substring(0, 150) + "..." : l.Description,
                    UserName = l.User?.DisplayName ?? "System",
                    Timestamp = l.Timestamp,
                    IPAddress = l.IPAddress,
                    IsSuspicious = l.Description.Contains("failed", StringComparison.OrdinalIgnoreCase) ||
                                   l.Description.Contains("attempt", StringComparison.OrdinalIgnoreCase) ||
                                   l.Description.Contains("unauthorized", StringComparison.OrdinalIgnoreCase)
                })
                .ToList();

            var model = new SecurityLogsViewModel
            {
                SecurityLogs = pagedLogs,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalLogs,
                TotalPages = (int)Math.Ceiling(totalLogs / (double)pageSize),
                TotalSecurityEvents = totalLogs,
                SuspiciousEvents = pagedLogs.Count(l => l.IsSuspicious),
                FailedLogins = securityLogs.Count(l => l.EventType.Contains("Login", StringComparison.OrdinalIgnoreCase) && 
                                                     l.Description.Contains("failed", StringComparison.OrdinalIgnoreCase)),
                UnauthorizedAccess = securityLogs.Count(l => l.EventType.Contains("Access", StringComparison.OrdinalIgnoreCase) ||
                                                            l.Description.Contains("unauthorized", StringComparison.OrdinalIgnoreCase))
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving security logs");
            return View("Error");
        }
    }

    [HttpPost]
    [Route("ClearOldLogs")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ClearOldLogs(int daysToKeep = 90)
    {
        try
        {
            await _loggingService.ClearOldLogsAsync(daysToKeep);

            await _loggingService.LogInfoAsync("LogsCleared",
                $"Logs older than {daysToKeep} days cleared by admin {GetCurrentUserEmail()}", GetCurrentUserId());

            TempData["SuccessMessage"] = $"Logs older than {daysToKeep} days cleared successfully!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing old logs");
            TempData["ErrorMessage"] = "Error clearing old logs.";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    [Route("DeleteLog/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteLog(long id)
    {
        try
        {
            await _logRepository.DeleteAsync((int)id);

            await _loggingService.LogInfoAsync("LogDeleted",
                $"Log {id} deleted by admin {GetCurrentUserEmail()}", GetCurrentUserId());

            TempData["SuccessMessage"] = "Log deleted successfully!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting log: {id}");
            TempData["ErrorMessage"] = "Error deleting log.";
            return RedirectToAction("Details", new { id });
        }
    }

    [HttpPost]
    [Route("DeleteLogsByType")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteLogsByType(string eventType, DateTime? olderThan = null)
    {
        try
        {
            var logs = await _logRepository.GetAllAsync();
            var logsToDelete = logs.Where(l => l.EventType == eventType);

            if (olderThan.HasValue)
                logsToDelete = logsToDelete.Where(l => l.Timestamp < olderThan.Value);

            int deletedCount = 0;
            foreach (var log in logsToDelete)
            {
                try
                {
                    await _logRepository.DeleteAsync((int)log.LogID);
                    deletedCount++;
                }
                catch
                {
                    // Continuar con el siguiente
                }
            }

            await _loggingService.LogInfoAsync("LogsDeletedByType",
                $"{deletedCount} logs of type '{eventType}' deleted by admin {GetCurrentUserEmail()}", GetCurrentUserId());

            TempData["SuccessMessage"] = $"{deletedCount} logs deleted successfully!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting logs by type: {eventType}");
            TempData["ErrorMessage"] = "Error deleting logs.";
            return RedirectToAction("Index");
        }
    }

    [HttpGet]
    [Route("Export")]
    public async Task<IActionResult> Export(
        DateTime? startDate = null,
        DateTime? endDate = null,
        string eventType = "",
        string status = "",
        string format = "csv")
    {
        try
        {
            startDate ??= DateTime.UtcNow.AddDays(-30);
            endDate ??= DateTime.UtcNow;

            var logs = await _logRepository.GetAllAsync();
            logs = logs.Where(l => l.Timestamp >= startDate && l.Timestamp <= endDate).ToList();

            if (!string.IsNullOrEmpty(eventType))
                logs = logs.Where(l => l.EventType == eventType).ToList();

            if (!string.IsNullOrEmpty(status))
                logs = logs.Where(l => l.Status == status).ToList();

            if (format == "json")
            {
                var jsonData = logs.Select(l => new
                {
                    l.LogID,
                    l.EventType,
                    l.Description,
                    User = l.User?.DisplayName,
                    UserID = l.UserID,
                    Timestamp = l.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                    l.IPAddress,
                    l.Status
                });

                return File(System.Text.Encoding.UTF8.GetBytes(
                    System.Text.Json.JsonSerializer.Serialize(jsonData, new System.Text.Json.JsonSerializerOptions { WriteIndented = true })),
                    "application/json", $"logs-export-{DateTime.UtcNow:yyyyMMdd}.json");
            }
            else
            {
                var csv = new System.Text.StringBuilder();
                csv.AppendLine("LogID,EventType,Description,User,UserID,Timestamp,IPAddress,Status");
                
                foreach (var log in logs)
                {
                    var description = log.Description?.Replace("\"", "\"\"") ?? "";
                    csv.AppendLine($"\"{log.LogID}\",\"{log.EventType}\",\"{description}\",\"{log.User?.DisplayName}\",\"{log.UserID}\",\"{log.Timestamp:yyyy-MM-dd HH:mm:ss}\",\"{log.IPAddress}\",\"{log.Status}\"");
                }

                return File(System.Text.Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"logs-export-{DateTime.UtcNow:yyyyMMdd}.csv");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting logs");
            TempData["ErrorMessage"] = "Error exporting logs.";
            return RedirectToAction("Index");
        }
    }

    private async Task<List<string>> GetAvailableEventTypes()
    {
        var logs = await _logRepository.GetAllAsync();
        return logs
            .Select(l => l.EventType)
            .Distinct()
            .OrderBy(et => et)
            .ToList();
    }

    private string FormatLogDescription(string description)
    {
        if (string.IsNullOrEmpty(description))
            return string.Empty;

        // Formatear la descripción para mejor visualización
        var formatted = description
            .Replace("\\n", "\n")
            .Replace("\\t", "\t")
            .Replace("\\r", "\r");

        return formatted;
    }

    private List<HourlyStatViewModel> GetLogsByHour(List<SystemLog> logs)
    {
        return logs
            .GroupBy(l => l.Timestamp.Hour)
            .Select(g => new HourlyStatViewModel
            {
                Hour = g.Key,
                Count = g.Count(),
                Percentage = (double)g.Count() / logs.Count * 100
            })
            .OrderBy(h => h.Hour)
            .ToList();
    }

    private List<DailyStatViewModel> GetLogsByDay(List<SystemLog> logs)
    {
        return logs
            .GroupBy(l => l.Timestamp.Date)
            .Select(g => new DailyStatViewModel
            {
                Date = g.Key,
                Count = g.Count(),
                InfoCount = g.Count(l => l.Status?.ToUpper() == "INFO"),
                WarningCount = g.Count(l => l.Status?.ToUpper() == "WARNING"),
                ErrorCount = g.Count(l => l.Status?.ToUpper() == "ERROR")
            })
            .OrderBy(d => d.Date)
            .ToList();
    }

    private List<ErrorTrendViewModel> GetErrorTrend(List<SystemLog> logs)
    {
        var errorLogs = logs.Where(l => l.IsError).ToList();
        
        return errorLogs
            .GroupBy(l => l.Timestamp.Date)
            .Select(g => new ErrorTrendViewModel
            {
                Date = g.Key,
                ErrorCount = g.Count(),
                UniqueErrors = g.Select(l => l.EventType).Distinct().Count()
            })
            .OrderBy(e => e.Date)
            .ToList();
    }

    private List<BusyHourViewModel> GetBusiestHours(List<SystemLog> logs)
    {
        return logs
            .GroupBy(l => l.Timestamp.Hour)
            .Select(g => new BusyHourViewModel
            {
                Hour = g.Key,
                TotalLogs = g.Count(),
                AvgLogsPerDay = (double)g.Count() / logs.Select(l => l.Timestamp.Date).Distinct().Count()
            })
            .OrderByDescending(b => b.TotalLogs)
            .Take(5)
            .ToList();
    }
}