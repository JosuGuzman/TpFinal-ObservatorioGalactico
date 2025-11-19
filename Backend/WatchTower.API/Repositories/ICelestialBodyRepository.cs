using WatchTower.API.Models.Entities;
using WatchTower.API.Models.DTOs;

namespace WatchTower.API.Repositories;

public interface ICelestialBodyRepository
{
    Task<IEnumerable<CelestialBodyResponse>> SearchCelestialBodiesAsync(CelestialBodySearchRequest request);
    Task<CelestialBody?> GetByIdAsync(int id);
    Task<IEnumerable<CelestialBody>> GetRecentBodiesAsync(int count);
}