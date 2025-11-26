// DTOs/UserDtos.cs
namespace WatchTower.Core.DTOs;

public class UserUpdateRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
}

public class UserStatisticsResponse
{
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int TotalDiscoveries { get; set; }
    public int VerifiedDiscoveries { get; set; }
    public int ObjectsExplored { get; set; }
    public int TotalFavorites { get; set; }
    public int TotalComments { get; set; }
}

public class ChangePasswordRequest
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class ResetPasswordRequest
{
    public string Email { get; set; } = string.Empty;
}