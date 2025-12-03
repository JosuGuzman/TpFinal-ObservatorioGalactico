namespace Observatorio.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
public class PlanetsController : BaseApiController
{
    private readonly IAstronomicalDataService _astronomicalService;
    private readonly ILoggingService _loggingService;

    public PlanetsController(IAstronomicalDataService astronomicalService, ILoggingService loggingService)
    {
        _astronomicalService = astronomicalService;
        _loggingService = loggingService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllPlanets([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var planets = await _astronomicalService.GetAllPlanetsAsync();
            
            var pagedPlanets = planets
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return SuccessResponse(new
            {
                data = pagedPlanets,
                page,
                pageSize,
                totalCount = planets.Count(),
                totalPages = (int)Math.Ceiling((double)planets.Count() / pageSize)
            });
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving planets", ex);
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPlanetById(int id)
    {
        try
        {
            var planet = await _astronomicalService.GetPlanetByIdAsync(id);
            
            if (planet == null)
                return NotFoundResponse($"Planet with ID {id} not found");

            return SuccessResponse(planet);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving planet", ex);
        }
    }

    [HttpGet("habitable")]
    [AllowAnonymous]
    public async Task<IActionResult> GetHabitablePlanets([FromQuery] double minHabitability = 0.7)
    {
        try
        {
            var planets = await _astronomicalService.GetHabitablesPlanetsAsync();
            var habitablePlanets = planets.Where(p => p.HabitabilityScore >= minHabitability);
            
            return SuccessResponse(habitablePlanets);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving habitable planets", ex);
        }
    }

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> SearchPlanets([FromQuery] string query, [FromQuery] int limit = 20)
    {
        try
        {
            var planets = await _astronomicalService.SearchPlanetsAsync(query);
            var results = planets.Take(limit);
            
            return SuccessResponse(results);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error searching planets", ex);
        }
    }

    [HttpPost("calculate/habitability")]
    [AllowAnonymous]
    public async Task<IActionResult> CalculateHabitability([FromBody] HabitabilityRequest request)
    {
        try
        {
            var habitability = await _astronomicalService.CalculateHabitability(
                request.Temperature, 
                request.DistanceAU, 
                request.Mass, 
                request.Radius);
            
            var category = habitability >= 0.7 ? "Potentially Habitable" : 
                          habitability >= 0.4 ? "Marginally Habitable" : 
                          "Not Habitable";
            
            return SuccessResponse(new 
            { 
                habitabilityScore = habitability,
                category,
                description = $"Planet with habitability score of {habitability:F2}"
            });
        }
        catch (Exception ex)
        {
            return BadRequestResponse(ex.Message);
        }
    }

    [HttpGet("stats")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPlanetStats()
    {
        try
        {
            var planets = await _astronomicalService.GetAllPlanetsAsync();
            var habitablePlanets = planets.Where(p => p.HabitabilityScore >= 0.7);
            
            var stats = new
            {
                totalPlanets = planets.Count(),
                habitablePlanets = habitablePlanets.Count(),
                averageHabitability = planets.Average(p => p.HabitabilityScore ?? 0),
                planetTypes = planets
                    .GroupBy(p => p.PlanetType)
                    .Select(g => new
                    {
                        type = g.Key.ToString(),
                        count = g.Count()
                    })
            };
            
            return SuccessResponse(stats);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving planet stats", ex);
        }
    }

    // Clase auxiliar
    public class HabitabilityRequest
    {
        public double Temperature { get; set; }
        public double DistanceAU { get; set; }
        public double Mass { get; set; }
        public double Radius { get; set; }
    }
}

// Extensiones para IAstronomicalDataService
public static class PlanetExtensions
{
    public static Task<IEnumerable<Planet>> GetAllPlanetsAsync(this IAstronomicalDataService service)
    {
        // Implementación simple
        return Task.FromResult(Enumerable.Empty<Planet>());
    }
}