namespace Observatorio.Mvc.Models.Admin;

public class ErrorLogsViewModel
{
    public List<ErrorLogViewModel> ErrorLogs { get; set; } = new();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int TotalErrors { get; set; }
    public int UniqueErrors { get; set; }
    public int Last24Hours { get; set; }
    public string MostCommonError { get; set; }
}