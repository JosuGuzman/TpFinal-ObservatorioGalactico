namespace Observatorio.Core.Entities.System;

public class SystemLog
{
    public long LogID { get; set; }
    public int? UserID { get; set; }
    public string EventType { get; set; }
    public string Description { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string IPAddress { get; set; }
    public string Status { get; set; }
    
    public virtual User.User User { get; set; }
    
    public bool IsSuccess => Status?.ToLower() == "ok" || Status?.ToLower() == "success";
    public bool IsError => Status?.ToLower() == "error" || Status?.ToLower() == "failed";
}