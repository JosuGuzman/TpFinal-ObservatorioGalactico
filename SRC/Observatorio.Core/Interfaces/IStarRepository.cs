namespace Observatorio.Core.Interfaces;

public interface IStarRepository : IRepository<Star>
{
    Task<IEnumerable<Star>> GetByGalaxyAsync(int galaxyId);
    Task<IEnumerable<Star>> GetBySpectralTypeAsync(string spectralType);
    Task<IEnumerable<Star>> SearchByNameAsync(string name);
    Task<IEnumerable<Star>> GetNearbyAsync(double ra, double dec, double radius);
    Task<int> CountByGalaxyAsync(int galaxyId);
}