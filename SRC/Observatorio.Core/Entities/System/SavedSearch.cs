namespace Observatorio.Core.Entities.System
{
    public class SavedSearch
    {
        public int SavedSearchID { get; set; }
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Criteria { get; set; } // JSON
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public virtual User.User User { get; set; }
    }
}