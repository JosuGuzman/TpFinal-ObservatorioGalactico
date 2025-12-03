namespace Observatorio.Core.Interfaces;

public interface IDiscoveryRepository : IRepository<Discovery>
{
    Task<IEnumerable<Discovery>> GetByReporterAsync(int userId);
    Task<IEnumerable<Discovery>> GetByStateAsync(string state);
    Task<IEnumerable<Discovery>> GetPendingAsync();
    Task UpdateStateAsync(int discoveryId, string state);
    Task<int> CountByStateAsync(string state);
    Task<IEnumerable<Discovery>> GetTopRatedAsync(int limit);
}