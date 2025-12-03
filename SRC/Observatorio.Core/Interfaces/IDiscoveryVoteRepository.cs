namespace Observatorio.Core.Interfaces;

public interface IDiscoveryVoteRepository : IRepository<DiscoveryVote>
{
    Task<DiscoveryVote> GetByUserAndDiscoveryAsync(int userId, int discoveryId);
    Task<bool> HasVotedAsync(int userId, int discoveryId);
    Task<int> GetUpvotesCountAsync(int discoveryId);
    Task<int> GetDownvotesCountAsync(int discoveryId);
    Task DeleteByDiscoveryAsync(int discoveryId);
}