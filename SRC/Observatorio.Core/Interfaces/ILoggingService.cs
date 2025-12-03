namespace Observatorio.Core.Interfaces;

public interface ILoggingService
{
    Task LogInfoAsync(string eventType, string description, int? userId = null, 
                     string ipAddress = null);
    Task LogWarningAsync(string eventType, string description, int? userId = null, 
                       string ipAddress = null);
    Task LogErrorAsync(string eventType, string description, int? userId = null, 
                      string ipAddress = null, Exception exception = null);
    Task LogSecurityAsync(string eventType, string description, int? userId = null, 
                        string ipAddress = null);
    
    Task<IEnumerable<SystemLog>> GetLogsAsync(DateTime? startDate = null, DateTime? endDate = null, 
                                             string eventType = null, int? userId = null);
    Task ClearOldLogsAsync(int daysToKeep = 90);
}