namespace Observatorio.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
public class AuthController : BaseApiController
{
    private readonly IUserService _userService;
    private readonly IAuthenticationService _authService;
    private readonly ILoggingService _loggingService;

    public AuthController(
        IUserService userService, 
        IAuthenticationService authService, 
        ILoggingService loggingService)
    {
        _userService = userService;
        _authService = authService;
        _loggingService = loggingService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] CreateUserRequest request)
    {
        try
        {
            var user = await _userService.RegisterAsync(
                request.Email, 
                request.UserName, 
                request.Password, 
                request.RoleID);

            var token = await _authService.GenerateJwtTokenAsync(
                user.UserID, 
                user.Email, 
                user.Role.RoleName);

            var response = new LoginResponse
            {
                UserID = user.UserID,
                Email = user.Email,
                UserName = user.UserName,
                DisplayName = user.DisplayName,
                Role = user.Role.RoleName,
                Token = token,
                TokenExpiration = DateTime.UtcNow.AddDays(7),
                LastLogin = DateTime.UtcNow
            };

            return CreatedResponse("/api/v1/auth/login", response, "User registered successfully");
        }
        catch (Exception ex)
        {
            return BadRequestResponse(ex.Message);
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var user = await _userService.AuthenticateAsync(request.Email, request.Password);
            
            if (user == null)
                return UnauthorizedResponse("Invalid credentials");

            var token = await _authService.GenerateJwtTokenAsync(
                user.UserID, 
                user.Email, 
                user.Role.RoleName);

            var response = new LoginResponse
            {
                UserID = user.UserID,
                Email = user.Email,
                UserName = user.UserName,
                DisplayName = user.DisplayName,
                Role = user.Role.RoleName,
                Token = token,
                TokenExpiration = DateTime.UtcNow.AddDays(7),
                LastLogin = DateTime.UtcNow
            };

            await _loggingService.LogInfoAsync("Login", $"User {user.Email} logged in", user.UserID);

            return SuccessResponse(response, "Login successful");
        }
        catch (Exception ex)
        {
            return BadRequestResponse(ex.Message);
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var userId = GetCurrentUserId();
            await _loggingService.LogInfoAsync("Logout", $"User {GetCurrentUserEmail()} logged out", userId);
            
            return SuccessResponse(new { message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            return BadRequestResponse(ex.Message);
        }
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _userService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
            
            return SuccessResponse(new { message = "Password changed successfully" });
        }
        catch (Exception ex)
        {
            return BadRequestResponse(ex.Message);
        }
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        try
        {
            await _userService.ResetPasswordAsync(request.Email);
            
            return SuccessResponse(new { message = "Password reset instructions sent to email" });
        }
        catch (Exception ex)
        {
            return BadRequestResponse(ex.Message);
        }
    }

    [HttpPost("refresh-token")]
    [Authorize]
    public async Task<IActionResult> RefreshToken()
    {
        try
        {
            var userId = GetCurrentUserId();
            var user = await _userService.GetByIdAsync(userId);
            
            if (user == null)
                return UnauthorizedResponse("User not found");

            var token = await _authService.GenerateJwtTokenAsync(
                user.UserID, 
                user.Email, 
                user.Role.RoleName);

            return SuccessResponse(new { token }, "Token refreshed successfully");
        }
        catch (Exception ex)
        {
            return BadRequestResponse(ex.Message);
        }
    }

    // Clases auxiliares para las peticiones
    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }

    public class ResetPasswordRequest
    {
        public string Email { get; set; }
    }
}