using WatchTower.API.Models.Entities;
using WatchTower.API.Models.DTOs;

namespace WatchTower.API.Repositories;

public interface IDiscoveryRepository
{
    Task<IEnumerable<DiscoveryResponse>> GetDiscoveriesAsync(string? searchTerm = null, string? status = null);
    Task<Discovery?> GetByIdAsync(int id);
    Task<int> AddDiscoveryAsync(Discovery discovery);
    Task UpdateDiscoveryAsync(Discovery discovery);
    Task<bool> AddVoteAsync(int discoveryId, int userId, string voteType);
}