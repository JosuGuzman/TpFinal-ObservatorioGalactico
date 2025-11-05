using Observatorio.Core.Entities;
using Observatorio.Core.Models;

namespace Observatorio.Core.Interfaces
{
    public interface IRepoCelestialBody
    {
        Task<CelestialBody> GetByIdAsync(int id);
        Task<IEnumerable<CelestialBody>> GetAllAsync();
        Task<IEnumerable<CelestialBody>> SearchAsync(SearchRequest request);
        Task<int> CreateAsync(CelestialBody body);
        Task UpdateAsync(CelestialBody body);
        Task DeleteAsync(int id);
    }
}
