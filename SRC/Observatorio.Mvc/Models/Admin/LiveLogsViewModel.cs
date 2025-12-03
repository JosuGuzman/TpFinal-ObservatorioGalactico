namespace Observatorio.Mvc.Models.Admin;

public class LiveLogsViewModel
{
    public int LastLogId { get; set; }
    public int RefreshInterval { get; set; } = 5000;
}