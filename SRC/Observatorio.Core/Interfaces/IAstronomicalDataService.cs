namespace Observatorio.Core.Interfaces;

public interface IAstronomicalDataService
{
    Task<IEnumerable<Galaxy>> GetAllGalaxiesAsync();
    Task<Galaxy> GetGalaxyByIdAsync(int id);
    Task<IEnumerable<Galaxy>> SearchGalaxiesAsync(string query);
    Task<IEnumerable<Galaxy>> GetGalaxiesByTypeAsync(string type);
    Task<int> GetGalaxiesCountAsync();
    
    Task<IEnumerable<Star>> GetStarsByGalaxyAsync(int galaxyId);
    Task<Star> GetStarByIdAsync(int id);
    Task<IEnumerable<Star>> SearchStarsAsync(string query);
    Task<IEnumerable<Star>> GetStarsBySpectralTypeAsync(string spectralType);
    
    Task<IEnumerable<Planet>> GetPlanetsByStarAsync(int starId);
    Task<Planet> GetPlanetByIdAsync(int id);
    Task<IEnumerable<Planet>> SearchPlanetsAsync(string query);
    Task<IEnumerable<Planet>> GetHabitablesPlanetsAsync();
    
    Task<double> CalculateAngularDistance(double ra1, double dec1, double ra2, double dec2);
    Task<double> CalculateHabitability(double temp, double distanceAU, double mass, double radius);
    Task<double> CalculateAbsoluteMagnitude(double apparentMagnitude, double distanceLy);
    
    Task<IEnumerable<object>> SearchAllAsync(string query, int limit = 50);
    Task<IEnumerable<object>> GetNearbyObjectsAsync(double ra, double dec, double radius, int limit = 20);
}