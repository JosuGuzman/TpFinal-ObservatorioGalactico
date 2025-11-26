namespace WatchTower.Core.Interfaces.Repositories;

public interface ICelestialBodyRepository
{
    Task<CelestialBody?> GetByIdAsync(int id);
    Task<IEnumerable<CelestialBody>> SearchAsync(CelestialBodySearchRequest request);
    Task<int> CreateAsync(CelestialBody body);
    Task UpdateAsync(CelestialBody body);
    Task<bool> VerifyBodyAsync(int bodyId, int verifiedBy);
    Task<IEnumerable<CelestialBody>> GetRecentBodiesAsync(int count);
    Task<int> GetTotalCountAsync();
}