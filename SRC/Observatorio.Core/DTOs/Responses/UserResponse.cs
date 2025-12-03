namespace Observatorio.Core.DTOs.Responses;

public class UserResponse
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
}