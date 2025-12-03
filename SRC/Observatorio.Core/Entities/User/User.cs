namespace Observatorio.Core.Entities.User;

public class User
{
    public int UserID { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string PasswordHash { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLogin { get; set; }
    public int RoleID { get; set; }
    public string ApiKey { get; set; }
    
    public virtual Role Role { get; set; }
    public virtual ICollection<UserFavorite> Favorites { get; set; } = new List<UserFavorite>();
    public virtual ICollection<ExplorationHistory> ExplorationHistory { get; set; } = new List<ExplorationHistory>();
    public virtual ICollection<SavedSearch> SavedSearches { get; set; } = new List<SavedSearch>();
    
    public string DisplayName => !string.IsNullOrEmpty(UserName) ? UserName : Email.Split('@')[0];
    public UserRole RoleEnum => (UserRole)RoleID;
    public bool IsAdmin => RoleEnum == UserRole.Admin;
    public bool IsAstronomer => RoleEnum >= UserRole.Astronomo;
    public bool IsResearcher => RoleEnum >= UserRole.Investigador;
}