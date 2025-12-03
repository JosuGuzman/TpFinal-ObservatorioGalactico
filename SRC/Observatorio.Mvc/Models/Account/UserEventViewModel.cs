namespace Observatorio.Mvc.Models.Account;

public class UserEventViewModel
{
    public int EventID { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public DateTime EventDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsUpcoming { get; set; }
}