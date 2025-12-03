namespace Observatorio.Mvc.Models.Account;

public class UserEventsViewModel
{
    public List<UserEventViewModel> Events { get; set; } = new();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}