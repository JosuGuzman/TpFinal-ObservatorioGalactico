namespace Observatorio.Mvc.Models.Admin;

public class AdminSettingsViewModel
{
    public string SiteTitle { get; set; }
    public string SiteDescription { get; set; }
    public bool MaintenanceMode { get; set; }
    public bool RegistrationEnabled { get; set; }
    public bool DiscoveryReportingEnabled { get; set; }
    public bool CommunityVotingEnabled { get; set; }
    public int MaxFileUploadSize { get; set; }
    public bool EmailNotifications { get; set; }
    public bool AnalyticsEnabled { get; set; }
    public int LogRetentionDays { get; set; }
}