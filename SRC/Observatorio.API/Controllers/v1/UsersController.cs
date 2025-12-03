namespace Observatorio.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class UsersController : BaseApiController
{
    private readonly IUserService _userService;
    private readonly ILoggingService _loggingService;

    public UsersController(IUserService userService, ILoggingService loggingService)
    {
        _userService = userService;
        _loggingService = loggingService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var users = await _userService.GetAllUsersAsync(page, pageSize);
            return SuccessResponse(users);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving users", ex);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        try
        {
            var user = await _userService.GetByIdAsync(id);
            
            if (user == null)
                return NotFoundResponse($"User with ID {id} not found");

            return SuccessResponse(user);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving user", ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            var user = await _userService.RegisterAsync(
                request.Email, 
                request.UserName, 
                request.Password, 
                request.RoleID);

            await _loggingService.LogInfoAsync("UserCreated", 
                $"User {user.Email} created by admin", 
                GetCurrentUserId());

            return CreatedResponse($"/api/v1/users/{user.UserID}", user);
        }
        catch (Exception ex)
        {
            return BadRequestResponse(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
        try
        {
            var user = await _userService.GetByIdAsync(id);
            
            if (user == null)
                return NotFoundResponse($"User with ID {id} not found");

            // Actualizar propiedades
            user.UserName = request.UserName ?? user.UserName;
            user.Email = request.Email ?? user.Email;
            user.RoleID = request.RoleID ?? user.RoleID;
            user.IsActive = request.IsActive ?? user.IsActive;

            await _userService.UpdateUserAsync(user);
            
            await _loggingService.LogInfoAsync("UserUpdated", 
                $"User {id} updated by admin", 
                GetCurrentUserId());

            return SuccessResponse(user, "User updated successfully");
        }
        catch (Exception ex)
        {
            return BadRequestResponse(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var user = await _userService.GetByIdAsync(id);
            
            if (user == null)
                return NotFoundResponse($"User with ID {id} not found");

            await _userService.DeleteUserAsync(id);
            
            await _loggingService.LogInfoAsync("UserDeleted", 
                $"User {id} deleted by admin", 
                GetCurrentUserId());

            return SuccessResponse(new { message = $"User {id} deleted successfully" });
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error deleting user", ex);
        }
    }

    [HttpPost("{id}/activate")]
    public async Task<IActionResult> ActivateUser(int id)
    {
        try
        {
            var user = await _userService.GetByIdAsync(id);
            
            if (user == null)
                return NotFoundResponse($"User with ID {id} not found");

            user.IsActive = true;
            await _userService.UpdateUserAsync(user);
            
            return SuccessResponse(new { message = $"User {id} activated successfully" });
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error activating user", ex);
        }
    }

    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> DeactivateUser(int id)
    {
        try
        {
            var user = await _userService.GetByIdAsync(id);
            
            if (user == null)
                return NotFoundResponse($"User with ID {id} not found");

            user.IsActive = false;
            await _userService.UpdateUserAsync(user);
            
            return SuccessResponse(new { message = $"User {id} deactivated successfully" });
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error deactivating user", ex);
        }
    }

    [HttpPost("{id}/generate-api-key")]
    public async Task<IActionResult> GenerateApiKey(int id)
    {
        try
        {
            var user = await _userService.GetByIdAsync(id);
            
            if (user == null)
                return NotFoundResponse($"User with ID {id} not found");

            await _userService.GenerateApiKeyAsync(id);
            
            // Obtener usuario actualizado
            user = await _userService.GetByIdAsync(id);
            
            return SuccessResponse(new { apiKey = user.ApiKey }, "API key generated successfully");
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error generating API key", ex);
        }
    }

    // Clases auxiliares
    public class UpdateUserRequest
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public int? RoleID { get; set; }
        public bool? IsActive { get; set; }
    }
}

// Extensión para IUserService (agregar en Core si no existe)
public interface IUserServiceExtended : IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync(int page = 1, int pageSize = 20);
}