namespace Observatorio.Mvc.Models.Account;

public class SavedSearchViewModel
{
    public int SavedSearchID { get; set; }
    public string Name { get; set; }
    public string Criteria { get; set; }
    public DateTime CreatedAt { get; set; }
}