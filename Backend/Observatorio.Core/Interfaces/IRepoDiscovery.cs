using Observatorio.Core.Models;
using Observatorio.Core.Entities;

namespace Observatorio.Core.Interfaces
{
    public interface IRepoDiscovery
    {
        Task<Discovery> GetByIdAsync(int id);
        Task<IEnumerable<Discovery>> GetAllAsync();
        Task<IEnumerable<Discovery>> SearchAsync(SearchRequest request);
        Task<int> CreateAsync(Discovery discovery);
        Task UpdateAsync(Discovery discovery);
        Task UpdateStatusAsync(int discoveryId, string status, int? verifiedBy);
        Task DeleteAsync(int id);
        Task<int> GetDiscoveryRatingAsync(int discoveryId);
        Task<bool> AddVoteAsync(int discoveryId, int userId, string voteType);
        Task<bool> RemoveVoteAsync(int discoveryId, int userId);
    }
}