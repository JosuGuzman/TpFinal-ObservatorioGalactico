namespace Observatorio.Mvc.Models.Admin;

public class ReportViewModel
{
    public string ReportType { get; set; }
    public DateTime GeneratedAt { get; set; }
    public string Content { get; set; }
}