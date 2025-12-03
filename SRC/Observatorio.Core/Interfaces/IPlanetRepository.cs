namespace Observatorio.Core.Interfaces;

public interface IPlanetRepository : IRepository<Planet>
{
    Task<IEnumerable<Planet>> GetByStarAsync(int starId);
    Task<IEnumerable<Planet>> GetByPlanetTypeAsync(string planetType);
    Task<IEnumerable<Planet>> GetHabitablesAsync(double minHabitability = 0.7);
    Task<IEnumerable<Planet>> SearchByNameAsync(string name);
    Task<int> CountByStarAsync(int starId);
    Task<int> CountByTypeAsync(string planetType);
}