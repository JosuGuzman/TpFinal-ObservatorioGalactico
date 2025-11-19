using WatchTower.API.Models.Entities;

namespace WatchTower.API.Services;

public interface ITokenService
{
    string GenerateToken(User user);
}
