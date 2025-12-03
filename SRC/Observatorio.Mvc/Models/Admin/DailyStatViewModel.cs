namespace Observatorio.Mvc.Models.Admin;

public class DailyStatViewModel
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
    public int InfoCount { get; set; }
    public int WarningCount { get; set; }
    public int ErrorCount { get; set; }
}