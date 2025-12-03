namespace Observatorio.Mvc.Models.Admin;

public class SystemLogDetailViewModel
{
    public long LogID { get; set; }
    public string EventType { get; set; }
    public string Description { get; set; }
    public int? UserID { get; set; }
    public string UserName { get; set; }
    public DateTime Timestamp { get; set; }
    public string IPAddress { get; set; }
    public string Status { get; set; }
    public bool IsSuccess { get; set; }
    public bool IsError { get; set; }
    public string RawData { get; set; }
}