namespace Observatorio.Core.Services;

public class AstronomicalDataService : IAstronomicalDataService
{
    private readonly IGalaxyRepository _galaxyRepository;
    private readonly IStarRepository _starRepository;
    private readonly IPlanetRepository _planetRepository;
    private readonly ILoggingService _loggingService;

    public AstronomicalDataService(
        IGalaxyRepository galaxyRepository,
        IStarRepository starRepository,
        IPlanetRepository planetRepository,
        ILoggingService loggingService)
    {
        _galaxyRepository = galaxyRepository;
        _starRepository = starRepository;
        _planetRepository = planetRepository;
        _loggingService = loggingService;
    }

    // Galaxias
    public async Task<IEnumerable<Galaxy>> GetAllGalaxiesAsync()
    {
        try
        {
            return await _galaxyRepository.GetAllAsync();
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync("AstronomicalData", 
                "Error getting all galaxies", null, null, ex);
            throw new AstronomicalDataException("Error retrieving galaxies", ex);
        }
    }

    public async Task<Galaxy> GetGalaxyByIdAsync(int id)
    {
        var galaxy = await _galaxyRepository.GetByIdAsync(id);
        if (galaxy == null)
            throw new NotFoundException("Galaxy", id);

        return galaxy;
    }

    public async Task<IEnumerable<Galaxy>> SearchGalaxiesAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return await GetAllGalaxiesAsync();

        return await _galaxyRepository.SearchByNameAsync(query);
    }

    public async Task<IEnumerable<Galaxy>> GetGalaxiesByTypeAsync(string type)
    {
        return await _galaxyRepository.GetByTypeAsync(type);
    }

    public async Task<int> GetGalaxiesCountAsync()
    {
        return await _galaxyRepository.CountAsync();
    }

    // Estrellas
    public async Task<IEnumerable<Star>> GetStarsByGalaxyAsync(int galaxyId)
    {
        return await _starRepository.GetByGalaxyAsync(galaxyId);
    }

    public async Task<Star> GetStarByIdAsync(int id)
    {
        var star = await _starRepository.GetByIdAsync(id);
        if (star == null)
            throw new NotFoundException("Star", id);

        return star;
    }

    public async Task<IEnumerable<Star>> SearchStarsAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return await _starRepository.GetAllAsync();

        return await _starRepository.SearchByNameAsync(query);
    }

    public async Task<IEnumerable<Star>> GetStarsBySpectralTypeAsync(string spectralType)
    {
        return await _starRepository.GetBySpectralTypeAsync(spectralType);
    }

    // Planetas
    public async Task<IEnumerable<Planet>> GetPlanetsByStarAsync(int starId)
    {
        return await _planetRepository.GetByStarAsync(starId);
    }

    public async Task<Planet> GetPlanetByIdAsync(int id)
    {
        var planet = await _planetRepository.GetByIdAsync(id);
        if (planet == null)
            throw new NotFoundException("Planet", id);

        return planet;
    }

    public async Task<IEnumerable<Planet>> SearchPlanetsAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return await _planetRepository.GetAllAsync();

        return await _planetRepository.SearchByNameAsync(query);
    }

    public async Task<IEnumerable<Planet>> GetHabitablesPlanetsAsync()
    {
        return await _planetRepository.GetHabitablesAsync();
    }

    // Cálculos
    public Task<double> CalculateAngularDistance(double ra1, double dec1, double ra2, double dec2)
    {
        if (!AstronomicalCalculations.IsValidCoordinates(ra1, dec1) || 
            !AstronomicalCalculations.IsValidCoordinates(ra2, dec2))
            throw new ValidationException("Invalid coordinates provided");

        var distance = AstronomicalCalculations.AngularDistance(ra1, dec1, ra2, dec2);
        return Task.FromResult(distance);
    }

    public Task<double> CalculateHabitability(double temp, double distanceAU, double mass, double radius)
    {
        var habitability = AstronomicalCalculations.CalculateHabitability(temp, distanceAU, mass, radius);
        return Task.FromResult(habitability);
    }

    public Task<double> CalculateAbsoluteMagnitude(double apparentMagnitude, double distanceLy)
    {
        if (distanceLy <= 0)
            throw new ValidationException("Distance must be greater than 0");

        var absoluteMagnitude = AstronomicalCalculations.AbsoluteMagnitude(apparentMagnitude, distanceLy);
        return Task.FromResult(absoluteMagnitude);
    }

    // Búsquedas combinadas
    public async Task<IEnumerable<object>> SearchAllAsync(string query, int limit = 50)
    {
        var results = new List<object>();

        if (string.IsNullOrWhiteSpace(query))
            return results;

        var galaxies = await _galaxyRepository.SearchByNameAsync(query);
        var stars = await _starRepository.SearchByNameAsync(query);
        var planets = await _planetRepository.SearchByNameAsync(query);

        results.AddRange(galaxies.Select(g => new { Type = "Galaxy", g.GalaxyID, g.Name, g.Description }));
        results.AddRange(stars.Select(s => new { Type = "Star", s.StarID, s.Name, s.Description }));
        results.AddRange(planets.Select(p => new { Type = "Planet", p.PlanetID, p.Name, p.Description }));

        return results.Take(limit);
    }

    public async Task<IEnumerable<object>> GetNearbyObjectsAsync(double ra, double dec, double radius, int limit = 20)
    {
        if (!AstronomicalCalculations.IsValidCoordinates(ra, dec))
            throw new ValidationException("Invalid coordinates");

        if (radius <= 0)
            throw new ValidationException("Radius must be greater than 0");

        var results = new List<object>();

        var nearbyGalaxies = await _galaxyRepository.GetNearbyAsync(ra, dec, radius);
        var nearbyStars = await _starRepository.GetNearbyAsync(ra, dec, radius);

        results.AddRange(nearbyGalaxies.Select(g => new 
        { 
            Type = "Galaxy", 
            g.GalaxyID, 
            g.Name, 
            g.RA, 
            g.Dec,
            Distance = AstronomicalCalculations.AngularDistance(ra, dec, g.RA, g.Dec)
        }));

        results.AddRange(nearbyStars.Select(s => new 
        { 
            Type = "Star", 
            s.StarID, 
            s.Name, 
            s.RA, 
            s.Dec,
            Distance = AstronomicalCalculations.AngularDistance(ra, dec, s.RA, s.Dec)
        }));

        return results.OrderBy(r => ((dynamic)r).Distance).Take(limit);
    }
}