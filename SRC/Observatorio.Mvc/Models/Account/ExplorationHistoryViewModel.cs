namespace Observatorio.Mvc.Models.Account;

public class ExplorationHistoryViewModel
{
    public long HistoryID { get; set; }
    public string ObjectType { get; set; }
    public int ObjectID { get; set; }
    public string ObjectName { get; set; }
    public DateTime AccessedAt { get; set; }
    public int? DurationSeconds { get; set; }
    public string SearchCriteria { get; set; }
}