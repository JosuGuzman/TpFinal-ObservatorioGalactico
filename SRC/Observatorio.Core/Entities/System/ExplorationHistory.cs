namespace Observatorio.Core.Entities.System;

public class ExplorationHistory
{
    public long HistoryID { get; set; }
    public int UserID { get; set; }
    public ObjectType ObjectType { get; set; }
    public int ObjectID { get; set; }
    public DateTime AccessedAt { get; set; } = DateTime.UtcNow;
    public int? DurationSeconds { get; set; }
    public string SearchCriteria { get; set; } // JSON
    
    public virtual User.User User { get; set; }
}