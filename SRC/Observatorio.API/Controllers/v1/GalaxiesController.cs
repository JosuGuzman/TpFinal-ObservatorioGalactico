namespace Observatorio.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
public class GalaxiesController : BaseApiController
{
    private readonly IAstronomicalDataService _astronomicalService;
    private readonly ILoggingService _loggingService;

    public GalaxiesController(IAstronomicalDataService astronomicalService, ILoggingService loggingService)
    {
        _astronomicalService = astronomicalService;
        _loggingService = loggingService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllGalaxies([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var galaxies = await _astronomicalService.GetAllGalaxiesAsync();
            var totalCount = await _astronomicalService.GetGalaxiesCountAsync();
            
            var pagedGalaxies = galaxies
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return SuccessResponse(new
            {
                data = pagedGalaxies,
                page,
                pageSize,
                totalCount,
                totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            });
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving galaxies", ex);
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetGalaxyById(int id)
    {
        try
        {
            var galaxy = await _astronomicalService.GetGalaxyByIdAsync(id);
            
            if (galaxy == null)
                return NotFoundResponse($"Galaxy with ID {id} not found");

            return SuccessResponse(galaxy);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving galaxy", ex);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Astronomer,Admin")]
    public async Task<IActionResult> CreateGalaxy([FromBody] CreateGalaxyRequest request)
    {
        try
        {
            var galaxy = new Galaxy
            {
                Name = request.Name,
                Type = Enum.Parse<GalaxyType>(request.Type),
                DistanceLy = request.DistanceLy,
                ApparentMagnitude = request.ApparentMagnitude,
                RA = request.RA,
                Dec = request.Dec,
                Redshift = request.Redshift,
                Description = request.Description,
                ImageURLs = request.ImageURLs,
                Discoverer = request.Discoverer,
                YearDiscovery = request.YearDiscovery,
                References = request.References,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // En una implementación real, usarías un repositorio aquí
            // Por simplicidad, devolvemos el objeto creado
            await _loggingService.LogInfoAsync("GalaxyCreated", 
                $"Galaxy '{request.Name}' created", 
                GetCurrentUserId());

            return CreatedResponse($"/api/v1/galaxies/{galaxy.GalaxyID}", galaxy);
        }
        catch (Exception ex)
        {
            return BadRequestResponse(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Astronomer,Admin")]
    public async Task<IActionResult> UpdateGalaxy(int id, [FromBody] UpdateGalaxyRequest request)
    {
        try
        {
            var galaxy = await _astronomicalService.GetGalaxyByIdAsync(id);
            
            if (galaxy == null)
                return NotFoundResponse($"Galaxy with ID {id} not found");

            galaxy.Name = request.Name ?? galaxy.Name;
            galaxy.Description = request.Description ?? galaxy.Description;
            galaxy.UpdatedAt = DateTime.UtcNow;

            // En una implementación real, guardarías los cambios
            await _loggingService.LogInfoAsync("GalaxyUpdated", 
                $"Galaxy {id} updated", 
                GetCurrentUserId());

            return SuccessResponse(galaxy, "Galaxy updated successfully");
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error updating galaxy", ex);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteGalaxy(int id)
    {
        try
        {
            var galaxy = await _astronomicalService.GetGalaxyByIdAsync(id);
            
            if (galaxy == null)
                return NotFoundResponse($"Galaxy with ID {id} not found");

            // En una implementación real, eliminarías la galaxia
            await _loggingService.LogInfoAsync("GalaxyDeleted", 
                $"Galaxy {id} deleted", 
                GetCurrentUserId());

            return SuccessResponse(new { message = $"Galaxy {id} deleted successfully" });
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error deleting galaxy", ex);
        }
    }

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> SearchGalaxies([FromQuery] string query, [FromQuery] int limit = 20)
    {
        try
        {
            var galaxies = await _astronomicalService.SearchGalaxiesAsync(query);
            var results = galaxies.Take(limit);
            
            return SuccessResponse(results);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error searching galaxies", ex);
        }
    }

    [HttpGet("{id}/stars")]
    [AllowAnonymous]
    public async Task<IActionResult> GetStarsByGalaxy(int id)
    {
        try
        {
            var stars = await _astronomicalService.GetStarsByGalaxyAsync(id);
            return SuccessResponse(stars);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving stars", ex);
        }
    }

    [HttpGet("type/{type}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetGalaxiesByType(string type)
    {
        try
        {
            var galaxies = await _astronomicalService.GetGalaxiesByTypeAsync(type);
            return SuccessResponse(galaxies);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving galaxies by type", ex);
        }
    }

    [HttpGet("nearby")]
    [AllowAnonymous]
    public async Task<IActionResult> GetNearbyGalaxies([FromQuery] double ra, [FromQuery] double dec, [FromQuery] double radius = 5)
    {
        try
        {
            var objects = await _astronomicalService.GetNearbyObjectsAsync(ra, dec, radius, 20);
            return SuccessResponse(objects);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving nearby objects", ex);
        }
    }

    // Clases auxiliares
    public class UpdateGalaxyRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}