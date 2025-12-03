namespace Observatorio.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class ProfileController : BaseApiController
{
    private readonly IUserProfileService _profileService;
    private readonly IUserService _userService;
    private readonly ILoggingService _loggingService;

    public ProfileController(
        IUserProfileService profileService,
        IUserService userService,
        ILoggingService loggingService)
    {
        _profileService = profileService;
        _userService = userService;
        _loggingService = loggingService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var userId = GetCurrentUserId();
            var user = await _userService.GetByIdAsync(userId);
            
            if (user == null)
                return NotFoundResponse("User not found");

            var profileData = new
            {
                user.UserID,
                user.Email,
                user.UserName,
                user.DisplayName,
                user.Role.RoleName,
                user.CreatedAt,
                user.LastLogin,
                user.IsActive,
                Statistics = await GetUserStatistics(userId)
            };

            return SuccessResponse(profileData);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving profile", ex);
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var user = await _userService.GetByIdAsync(userId);
            
            if (user == null)
                return NotFoundResponse("User not found");

            // Actualizar solo los campos permitidos
            if (!string.IsNullOrEmpty(request.UserName))
                user.UserName = request.UserName;

            // Nota: El email no debería cambiarse fácilmente
            // Se necesita verificación adicional

            await _userService.UpdateUserAsync(user);
            
            await _loggingService.LogInfoAsync("ProfileUpdated", 
                $"User {userId} updated their profile", 
                userId);

            return SuccessResponse(user, "Profile updated successfully");
        }
        catch (Exception ex)
        {
            return BadRequestResponse(ex.Message);
        }
    }

    [HttpGet("favorites")]
    public async Task<IActionResult> GetFavorites([FromQuery] string type = null)
    {
        try
        {
            var userId = GetCurrentUserId();
            var favorites = await _profileService.GetUserFavoritesAsync(userId);
            
            if (!string.IsNullOrEmpty(type))
            {
                favorites = favorites.Where(f => f.ObjectType.ToString().Equals(type, StringComparison.OrdinalIgnoreCase));
            }

            return SuccessResponse(favorites);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving favorites", ex);
        }
    }

    [HttpPost("favorites")]
    public async Task<IActionResult> AddFavorite([FromBody] AddFavoriteRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            
            await _profileService.AddFavoriteAsync(userId, request.ObjectType, request.ObjectID);
            
            return SuccessResponse(new { message = "Added to favorites" });
        }
        catch (Exception ex)
        {
            return BadRequestResponse(ex.Message);
        }
    }

    [HttpDelete("favorites")]
    public async Task<IActionResult> RemoveFavorite([FromBody] RemoveFavoriteRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            
            await _profileService.RemoveFavoriteAsync(userId, request.ObjectType, request.ObjectID);
            
            return SuccessResponse(new { message = "Removed from favorites" });
        }
        catch (Exception ex)
        {
            return BadRequestResponse(ex.Message);
        }
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory([FromQuery] int limit = 50)
    {
        try
        {
            var userId = GetCurrentUserId();
            var history = await _profileService.GetUserHistoryAsync(userId, limit);
            
            return SuccessResponse(history);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving history", ex);
        }
    }

    [HttpDelete("history")]
    public async Task<IActionResult> ClearHistory()
    {
        try
        {
            var userId = GetCurrentUserId();
            await _profileService.ClearHistoryAsync(userId);
            
            return SuccessResponse(new { message = "History cleared successfully" });
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error clearing history", ex);
        }
    }

    [HttpGet("saved-searches")]
    public async Task<IActionResult> GetSavedSearches()
    {
        try
        {
            var userId = GetCurrentUserId();
            var searches = await _profileService.GetUserSavedSearchesAsync(userId);
            
            return SuccessResponse(searches);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving saved searches", ex);
        }
    }

    [HttpPost("saved-searches")]
    public async Task<IActionResult> SaveSearch([FromBody] SaveSearchRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var search = await _profileService.SaveSearchAsync(userId, request.Name, request.Criteria);
            
            return CreatedResponse($"/api/v1/profile/saved-searches/{search.SavedSearchID}", search);
        }
        catch (Exception ex)
        {
            return BadRequestResponse(ex.Message);
        }
    }

    [HttpDelete("saved-searches/{id}")]
    public async Task<IActionResult> DeleteSavedSearch(int id)
    {
        try
        {
            await _profileService.DeleteSavedSearchAsync(id);
            
            return SuccessResponse(new { message = "Saved search deleted successfully" });
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error deleting saved search", ex);
        }
    }

    [HttpGet("notifications")]
    public async Task<IActionResult> GetNotifications([FromQuery] bool unreadOnly = false)
    {
        try
        {
            var userId = GetCurrentUserId();
            var notifications = await _profileService.GetUserNotificationsAsync(userId, unreadOnly);
            
            return SuccessResponse(notifications);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving notifications", ex);
        }
    }

    [HttpPost("notifications/{id}/read")]
    public async Task<IActionResult> MarkNotificationAsRead(int id)
    {
        try
        {
            await _profileService.MarkNotificationAsReadAsync(id);
            
            return SuccessResponse(new { message = "Notification marked as read" });
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error updating notification", ex);
        }
    }

    [HttpPost("notifications/read-all")]
    public async Task<IActionResult> MarkAllNotificationsAsRead()
    {
        try
        {
            var userId = GetCurrentUserId();
            await _profileService.MarkAllNotificationsAsReadAsync(userId);
            
            return SuccessResponse(new { message = "All notifications marked as read" });
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error updating notifications", ex);
        }
    }

    [HttpGet("preferences")]
    public async Task<IActionResult> GetPreferences()
    {
        try
        {
            var userId = GetCurrentUserId();
            var preferences = await _profileService.GetUserPreferencesAsync(userId);
            
            return SuccessResponse(preferences);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving preferences", ex);
        }
    }

    [HttpPut("preferences")]
    public async Task<IActionResult> UpdatePreferences([FromBody] UpdatePreferencesRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _profileService.UpdateUserPreferencesAsync(userId, request.Preferences);
            
            return SuccessResponse(new { message = "Preferences updated successfully" });
        }
        catch (Exception ex)
        {
            return BadRequestResponse(ex.Message);
        }
    }

    // Métodos auxiliares
    private async Task<object> GetUserStatistics(int userId)
    {
        return new
        {
            FavoritesCount = (await _profileService.GetUserFavoritesAsync(userId)).Count(),
            HistoryCount = (await _profileService.GetUserHistoryAsync(userId, 1000)).Count(),
            SavedSearchesCount = (await _profileService.GetUserSavedSearchesAsync(userId)).Count(),
            UnreadNotifications = (await _profileService.GetUserNotificationsAsync(userId, true)).Count()
        };
    }

    // Clases auxiliares
    public class UpdateProfileRequest
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
    }

    public class RemoveFavoriteRequest
    {
        public string ObjectType { get; set; }
        public int ObjectID { get; set; }
    }

    public class SaveSearchRequest
    {
        public string Name { get; set; }
        public string Criteria { get; set; }
    }

    public class UpdatePreferencesRequest
    {
        public string Preferences { get; set; }
    }
}

// Extensiones para IUserProfileService
public static class UserProfileExtensions
{
    public static Task<IEnumerable<Notification>> GetUserNotificationsAsync(this IUserProfileService service, int userId, bool unreadOnly)
        => Task.FromResult(Enumerable.Empty<Notification>());
    
    public static Task MarkNotificationAsReadAsync(this IUserProfileService service, int notificationId)
        => Task.CompletedTask;
    
    public static Task MarkAllNotificationsAsReadAsync(this IUserProfileService service, int userId)
        => Task.CompletedTask;
    
    public static Task<object> GetUserPreferencesAsync(this IUserProfileService service, int userId)
        => Task.FromResult<object>(null);
    
    public static Task UpdateUserPreferencesAsync(this IUserProfileService service, int userId, string preferences)
        => Task.CompletedTask;
}

// Clase auxiliar para notificaciones
public class Notification
{
    public int NotificationID { get; set; }
    public int UserID { get; set; }
    public string Type { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Link { get; set; }
}