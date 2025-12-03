namespace Observatorio.Core.Interfaces;

public interface IAuthenticationService
{
    Task<string> GenerateJwtTokenAsync(int userId, string email, string role);
    Task<bool> ValidateJwtTokenAsync(string token);
    Task<int?> GetUserIdFromTokenAsync(string token);
    Task<string> GenerateApiKeyAsync(int userId);
    Task<bool> ValidateApiKeyAsync(string apiKey);
    Task RevokeApiKeyAsync(int userId);
}