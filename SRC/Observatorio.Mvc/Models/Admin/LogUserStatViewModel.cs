namespace Observatorio.Mvc.Models.Admin;

public class LogUserStatViewModel
{
    public int UserID { get; set; }
    public string UserName { get; set; }
    public int Count { get; set; }
    public DateTime LastActivity { get; set; }
}