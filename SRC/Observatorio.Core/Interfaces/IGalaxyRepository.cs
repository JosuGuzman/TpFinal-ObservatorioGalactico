namespace Observatorio.Core.Interfaces;

public interface IGalaxyRepository : IRepository<Galaxy>
{
    Task<IEnumerable<Galaxy>> GetByTypeAsync(string type);
    Task<IEnumerable<Galaxy>> SearchByNameAsync(string name);
    Task<IEnumerable<Galaxy>> GetByDistanceRangeAsync(double minDistance, double maxDistance);
    Task<IEnumerable<Galaxy>> GetNearbyAsync(double ra, double dec, double radius);
    Task<int> CountByTypeAsync(string type);
}