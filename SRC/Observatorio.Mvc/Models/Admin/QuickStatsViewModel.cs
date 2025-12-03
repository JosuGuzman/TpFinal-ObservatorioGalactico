namespace Observatorio.Mvc.Models.Admin;

public class QuickStatsViewModel
{
    public DailyStatsViewModel Today { get; set; }
    public DailyStatsViewModel Yesterday { get; set; }
    public DailyStatsViewModel Last7Days { get; set; }
    public DailyStatsViewModel Last30Days { get; set; }
}