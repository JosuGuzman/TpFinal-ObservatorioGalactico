namespace WatchTower.Core.Interfaces.Services;

public interface IDiscoveryService
{
    Task<DiscoveryDetailResponse?> GetByIdAsync(int id);
    Task<PagedResult<DiscoveryResponse>> SearchAsync(DiscoverySearchRequest request);
    Task<DiscoveryResponse> CreateAsync(DiscoveryCreateRequest request, int reportedBy);
    Task<DiscoveryResponse?> UpdateAsync(int id, DiscoveryUpdateRequest request, int userId);
    Task<bool> VoteAsync(int discoveryId, int userId, VoteType voteType);
    Task<bool> UpdateStatusAsync(int discoveryId, DiscoveryStatus status, int verifiedBy);
    Task<IEnumerable<DiscoveryResponse>> GetUserDiscoveriesAsync(int userId);
}