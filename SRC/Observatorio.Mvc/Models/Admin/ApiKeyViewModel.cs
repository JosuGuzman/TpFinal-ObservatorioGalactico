namespace Observatorio.Mvc.Models.Admin;

public class ApiKeyViewModel
{
    public int UserID { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public string ApiKey { get; set; }
    public DateTime? LastLogin { get; set; }
    public bool IsActive { get; set; }
}