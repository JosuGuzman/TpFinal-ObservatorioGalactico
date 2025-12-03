namespace Observatorio.Mvc.Models.Admin;

public class SecurityLogsViewModel
{
    public List<SecurityLogViewModel> SecurityLogs { get; set; } = new();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int TotalSecurityEvents { get; set; }
    public int SuspiciousEvents { get; set; }
    public int FailedLogins { get; set; }
    public int UnauthorizedAccess { get; set; }
}