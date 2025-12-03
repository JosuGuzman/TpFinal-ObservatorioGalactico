namespace Observatorio.Core.Services;

public class LoggingService : ILoggingService
{
    private readonly ISystemLogRepository _logRepository;

    public LoggingService(ISystemLogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public async Task LogInfoAsync(string eventType, string description, int? userId = null, string ipAddress = null)
    {
        await LogAsync(eventType, description, userId, ipAddress, "OK");
    }

    public async Task LogWarningAsync(string eventType, string description, int? userId = null, string ipAddress = null)
    {
        await LogAsync(eventType, description, userId, ipAddress, "WARNING");
    }

    public async Task LogErrorAsync(string eventType, string description, int? userId = null, 
        string ipAddress = null, Exception exception = null)
    {
        if (exception != null)
        {
            description += $"\nException: {exception.Message}\nStackTrace: {exception.StackTrace}";
        }

        await LogAsync(eventType, description, userId, ipAddress, "ERROR");
    }

    public async Task LogSecurityAsync(string eventType, string description, int? userId = null, string ipAddress = null)
    {
        await LogAsync(eventType, description, userId, ipAddress, "SECURITY");
    }

    private async Task LogAsync(string eventType, string description, int? userId, string ipAddress, string status)
    {
        try
        {
            var log = new SystemLog
            {
                UserID = userId,
                EventType = eventType,
                Description = description,
                IPAddress = ipAddress,
                Status = status,
                Timestamp = DateTime.UtcNow
            };

            await _logRepository.AddAsync(log);
        }
        catch (Exception ex)
        {
            // Si falla el logging, no podemos hacer logging del error
            // En producción, podrías escribir en un archivo o usar otro sistema
            Console.WriteLine($"Error logging event: {eventType}. Error: {ex.Message}");
        }
    }

    public async Task<IEnumerable<SystemLog>> GetLogsAsync(DateTime? startDate = null, DateTime? endDate = null, 
        string eventType = null, int? userId = null)
    {
        var allLogs = await _logRepository.GetAllAsync();
        var filteredLogs = allLogs;

        if (startDate.HasValue)
            filteredLogs = filteredLogs.Where(l => l.Timestamp >= startDate.Value);

        if (endDate.HasValue)
            filteredLogs = filteredLogs.Where(l => l.Timestamp <= endDate.Value);

        if (!string.IsNullOrEmpty(eventType))
            filteredLogs = filteredLogs.Where(l => l.EventType == eventType);

        if (userId.HasValue)
            filteredLogs = filteredLogs.Where(l => l.UserID == userId);

        return filteredLogs.OrderByDescending(l => l.Timestamp);
    }

    public async Task ClearOldLogsAsync(int daysToKeep = 90)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);
        await _logRepository.ClearOldLogsAsync(cutoffDate);
    }
}