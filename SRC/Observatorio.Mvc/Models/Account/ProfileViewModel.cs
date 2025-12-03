namespace Observatorio.Mvc.Models.Account;

public class ProfileViewModel
{
    public int UserID { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string DisplayName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
    public string Role { get; set; }
    public string ApiKey { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsAstronomer { get; set; }
    public bool IsResearcher { get; set; }
}