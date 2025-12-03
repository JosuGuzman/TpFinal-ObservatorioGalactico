namespace Observatorio.Core.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly string _secretKey;
    private readonly int _tokenExpirationDays;

    public AuthenticationService(string secretKey, int tokenExpirationDays = 7)
    {
        _secretKey = secretKey;
        _tokenExpirationDays = tokenExpirationDays;
    }

    public Task<string> GenerateJwtTokenAsync(int userId, string email, string role)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
            Expires = DateTime.UtcNow.AddDays(_tokenExpirationDays),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Task.FromResult(tokenHandler.WriteToken(token));
    }

    public Task<bool> ValidateJwtTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Task<int?> GetUserIdFromTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return Task.FromResult<int?>(userId);
            }

            return Task.FromResult<int?>(null);
        }
        catch
        {
            return Task.FromResult<int?>(null);
        }
    }

    public Task<string> GenerateApiKeyAsync(int userId)
    {
        var apiKey = StringHelpers.GenerateRandomString(64);
        return Task.FromResult(apiKey);
    }

    public Task<bool> ValidateApiKeyAsync(string apiKey)
    {
        // En una implementación real, esto validaría contra la base de datos
        // Por ahora, solo validamos que tenga el formato correcto
        return Task.FromResult(!string.IsNullOrEmpty(apiKey) && apiKey.Length == 64);
    }

    public Task RevokeApiKeyAsync(int userId)
    {
        // En una implementación real, esto eliminaría la API key de la base de datos
        return Task.CompletedTask;
    }
}