namespace Observatorio.Core.DTOs.Responses;

public class LoginResponse
{
    public int UserID { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string DisplayName { get; set; }
    public string Role { get; set; }
    public string Token { get; set; }
    public DateTime TokenExpiration { get; set; }
    public DateTime LastLogin { get; set; }
}