namespace Observatorio.Mvc.Models.Admin;

public class ErrorTrendViewModel
{
    public DateTime Date { get; set; }
    public int ErrorCount { get; set; }
    public int UniqueErrors { get; set; }
}