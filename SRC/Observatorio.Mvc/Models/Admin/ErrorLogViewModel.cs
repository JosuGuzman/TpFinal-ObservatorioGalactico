namespace Observatorio.Mvc.Models.Admin;

public class ErrorLogViewModel
{
    public long LogID { get; set; }
    public string EventType { get; set; }
    public string Description { get; set; }
    public string UserName { get; set; }
    public DateTime Timestamp { get; set; }
    public string IPAddress { get; set; }
}