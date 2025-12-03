namespace Observatorio.Mvc.Models.Shared;

public class NotificationViewModel
{
    public string Type { get; set; } // success, info, warning, error
    public string Title { get; set; }
    public string Message { get; set; }
    public bool Dismissible { get; set; } = true;
    public int AutoHideDelay { get; set; } = 5000; // milliseconds
}