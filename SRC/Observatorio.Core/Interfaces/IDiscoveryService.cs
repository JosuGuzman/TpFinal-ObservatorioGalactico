namespace Observatorio.Core.Interfaces;

public interface IDiscoveryService
{
    Task<Discovery> CreateDiscoveryAsync(int reporterUserId, string objectType, string suggestedName, 
                                        double ra, double dec, string description, string attachments = null);
    Task<Discovery> GetDiscoveryByIdAsync(int id);
    Task<IEnumerable<Discovery>> GetAllDiscoveriesAsync();
    Task<IEnumerable<Discovery>> GetDiscoveriesByStateAsync(string state);
    Task<IEnumerable<Discovery>> GetUserDiscoveriesAsync(int userId);
    Task<Discovery> UpdateDiscoveryStateAsync(int discoveryId, string state);
    Task DeleteDiscoveryAsync(int discoveryId);
    
    Task VoteDiscoveryAsync(int discoveryId, int voterUserId, bool vote, string comment = null);
    Task RemoveVoteAsync(int discoveryId, int voterUserId);
    Task<bool> HasUserVotedAsync(int discoveryId, int userId);
    Task<int> GetUpvotesCountAsync(int discoveryId);
    Task<int> GetDownvotesCountAsync(int discoveryId);
    Task<double> GetApprovalRateAsync(int discoveryId);
    
    Task<bool> ValidateDiscoveryAsync(int discoveryId, int astronomerId);
    Task<bool> RejectDiscoveryAsync(int discoveryId, int astronomerId, string reason);
}