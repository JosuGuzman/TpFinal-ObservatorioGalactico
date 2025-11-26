namespace WatchTower.Core.Interfaces.Repositories;

public interface IDiscoveryRepository
{
    Task<Discovery?> GetByIdAsync(int id);
    Task<IEnumerable<Discovery>> SearchAsync(DiscoverySearchRequest request);
    Task<int> CreateAsync(Discovery discovery);
    Task UpdateAsync(Discovery discovery);
    Task<bool> AddVoteAsync(int discoveryId, int userId, VoteType voteType);
    Task<bool> UpdateStatusAsync(int discoveryId, DiscoveryStatus status, int verifiedBy);
    Task<IEnumerable<Discovery>> GetUserDiscoveriesAsync(int userId);
    Task<int> GetDiscoveryCountAsync(DiscoveryStatus? status = null);
    Task<bool> HasUserVotedAsync(int discoveryId, int userId);
}