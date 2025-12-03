namespace Observatorio.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
[ApiVersion("1.0")]
public abstract class BaseApiController : ControllerBase
{
    protected IActionResult SuccessResponse<T>(T data, string message = "Success")
    {
        return Ok(ApiResponse<T>.SuccessResponse(data, message));
    }

    protected IActionResult CreatedResponse<T>(string uri, T data, string message = "Resource created")
    {
        return Created(uri, ApiResponse<T>.SuccessResponse(data, message));
    }

    protected IActionResult BadRequestResponse(string message, object data = null)
    {
        return BadRequest(ApiResponse<object>.ErrorResponse(message, data));
    }

    protected IActionResult NotFoundResponse(string message)
    {
        return NotFound(ApiResponse<object>.ErrorResponse(message));
    }

    protected IActionResult UnauthorizedResponse(string message = "Unauthorized")
    {
        return Unauthorized(ApiResponse<object>.ErrorResponse(message));
    }

    protected IActionResult ForbiddenResponse(string message = "Forbidden")
    {
        return StatusCode(403, ApiResponse<object>.ErrorResponse(message));
    }

    protected IActionResult InternalErrorResponse(string message = "Internal server error", Exception ex = null)
    {
        // En producción, no exponer detalles del error
        var errorDetails = ex != null && HttpContext.User.IsInRole("Admin") ? ex.Message : null;
        return StatusCode(500, ApiResponse<object>.ErrorResponse(message, errorDetails));
    }

    protected int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
    }

    protected string GetCurrentUserEmail()
    {
        var emailClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Email);
        return emailClaim?.Value;
    }

    protected string GetCurrentUserRole()
    {
        var roleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role);
        return roleClaim?.Value;
    }

    protected bool IsAdmin() => GetCurrentUserRole() == "Admin";
    protected bool IsAstronomer() => GetCurrentUserRole() == "Astronomer" || IsAdmin();
    protected bool IsResearcher() => GetCurrentUserRole() == "Researcher" || IsAstronomer();
}