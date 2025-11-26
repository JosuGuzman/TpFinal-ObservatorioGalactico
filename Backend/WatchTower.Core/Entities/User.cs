namespace WatchTower.Core.Entities;

public class User
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Visitor;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
    
    // Navigation properties
    public virtual ICollection<Discovery> Discoveries { get; set; } = new List<Discovery>();
    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
}