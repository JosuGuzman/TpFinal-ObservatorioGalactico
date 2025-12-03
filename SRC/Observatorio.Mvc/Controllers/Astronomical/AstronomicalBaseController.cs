using Microsoft.AspNetCore.Authorization;
using Observatorio.Core.Interfaces;

namespace Observatorio.Mvc.Controllers.Astronomical;

[Authorize]
public class AstronomicalBaseController : BaseController
{
    protected readonly IAstronomicalDataService _astronomicalService;
    protected readonly ILogger _logger;

    public AstronomicalBaseController(
        IAstronomicalDataService astronomicalService,
        ILogger logger)
    {
        _astronomicalService = astronomicalService;
        _logger = logger;
    }

    protected async Task<(bool IsValid, string Message)> ValidateCoordinates(double ra, double dec)
    {
        if (ra < 0 || ra > 360)
            return (false, "Ascensión recta debe estar entre 0 y 360 grados");
        
        if (dec < -90 || dec > 90)
            return (false, "Declinación debe estar entre -90 y 90 grados");
        
        return (true, string.Empty);
    }

    protected async Task<(bool IsValid, string Message)> ValidateDistance(double distance)
    {
        if (distance < 0)
            return (false, "La distancia debe ser un valor positivo");
        
        return (true, string.Empty);
    }
}