namespace Observatorio.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
public class StarsController : BaseApiController
{
    private readonly IAstronomicalDataService _astronomicalService;
    private readonly ILoggingService _loggingService;

    public StarsController(IAstronomicalDataService astronomicalService, ILoggingService loggingService)
    {
        _astronomicalService = astronomicalService;
        _loggingService = loggingService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllStars([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var stars = await _astronomicalService.GetAllStarsAsync();
            
            var pagedStars = stars
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return SuccessResponse(new
            {
                data = pagedStars,
                page,
                pageSize,
                totalCount = stars.Count(),
                totalPages = (int)Math.Ceiling((double)stars.Count() / pageSize)
            });
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving stars", ex);
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetStarById(int id)
    {
        try
        {
            var star = await _astronomicalService.GetStarByIdAsync(id);
            
            if (star == null)
                return NotFoundResponse($"Star with ID {id} not found");

            return SuccessResponse(star);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving star", ex);
        }
    }

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> SearchStars([FromQuery] string query, [FromQuery] int limit = 20)
    {
        try
        {
            var stars = await _astronomicalService.SearchStarsAsync(query);
            var results = stars.Take(limit);
            
            return SuccessResponse(results);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error searching stars", ex);
        }
    }

    [HttpGet("spectral/{spectralType}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetStarsBySpectralType(string spectralType)
    {
        try
        {
            var stars = await _astronomicalService.GetStarsBySpectralTypeAsync(spectralType);
            return SuccessResponse(stars);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving stars by spectral type", ex);
        }
    }

    [HttpGet("{id}/planets")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPlanetsByStar(int id)
    {
        try
        {
            var planets = await _astronomicalService.GetPlanetsByStarAsync(id);
            return SuccessResponse(planets);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving planets", ex);
        }
    }

    [HttpGet("nearby")]
    [AllowAnonymous]
    public async Task<IActionResult> GetNearbyStars([FromQuery] double ra, [FromQuery] double dec, [FromQuery] double radius = 2)
    {
        try
        {
            var stars = await _astronomicalService.GetNearbyStarsAsync(ra, dec, radius);
            return SuccessResponse(stars);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving nearby stars", ex);
        }
    }

    [HttpPost("calculate/angular-distance")]
    [AllowAnonymous]
    public async Task<IActionResult> CalculateAngularDistance([FromBody] AngularDistanceRequest request)
    {
        try
        {
            var distance = await _astronomicalService.CalculateAngularDistance(
                request.RA1, request.Dec1, request.RA2, request.Dec2);
            
            return SuccessResponse(new { angularDistance = distance, unit = "degrees" });
        }
        catch (Exception ex)
        {
            return BadRequestResponse(ex.Message);
        }
    }

    [HttpPost("calculate/absolute-magnitude")]
    [AllowAnonymous]
    public async Task<IActionResult> CalculateAbsoluteMagnitude([FromBody] AbsoluteMagnitudeRequest request)
    {
        try
        {
            var absoluteMagnitude = await _astronomicalService.CalculateAbsoluteMagnitude(
                request.ApparentMagnitude, request.DistanceLy);
            
            return SuccessResponse(new { absoluteMagnitude });
        }
        catch (Exception ex)
        {
            return BadRequestResponse(ex.Message);
        }
    }

    // Clases auxiliares
    public class AngularDistanceRequest
    {
        public double RA1 { get; set; }
        public double Dec1 { get; set; }
        public double RA2 { get; set; }
        public double Dec2 { get; set; }
    }

    public class AbsoluteMagnitudeRequest
    {
        public double ApparentMagnitude { get; set; }
        public double DistanceLy { get; set; }
    }
}

// Extensiones para IAstronomicalDataService (agregar si no existen)
public static class AstronomicalDataServiceExtensions
{
    public static Task<IEnumerable<Star>> GetAllStarsAsync(this IAstronomicalDataService service)
    {
        // Implementación simple - en realidad deberías tener un método en el servicio
        return Task.FromResult(Enumerable.Empty<Star>());
    }

    public static Task<IEnumerable<Star>> GetNearbyStarsAsync(this IAstronomicalDataService service, double ra, double dec, double radius)
    {
        // Implementación simple
        return Task.FromResult(Enumerable.Empty<Star>());
    }
}