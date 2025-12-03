namespace Observatorio.Mvc.Models.Admin;

public class ActivityViewModel
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<ActivityLogViewModel> ActivityLogs { get; set; } = new();
    public int TotalEvents { get; set; }
    public int SuccessEvents { get; set; }
    public int ErrorEvents { get; set; }
    public int WarningEvents { get; set; }
    public int SecurityEvents { get; set; }
}