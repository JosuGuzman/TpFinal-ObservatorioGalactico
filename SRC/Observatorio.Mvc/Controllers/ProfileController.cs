using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Observatorio.Core.Entities.User;
using Observatorio.Core.Interfaces;
using Observatorio.Mvc.Models.Account;
using System.Security.Claims;

namespace Observatorio.Mvc.Controllers;

[Authorize]
public class ProfileController : BaseController
{
    private readonly IUserService _userService;
    private readonly IUserProfileService _userProfileService;
    private readonly IDiscoveryService _discoveryService;
    private readonly IContentService _contentService;
    private readonly ILoggingService _loggingService;

    public ProfileController(
        IUserService userService,
        IUserProfileService userProfileService,
        IDiscoveryService discoveryService,
        IContentService contentService,
        ILoggingService loggingService)
    {
        _userService = userService;
        _userProfileService = userProfileService;
        _discoveryService = discoveryService;
        _contentService = contentService;
        _loggingService = loggingService;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var userId = GetCurrentUserId();
            var user = await _userService.GetByIdAsync(userId);
            
            if (user == null)
                return NotFound();

            var model = new ProfileViewModel
            {
                UserID = user.UserID,
                Email = user.Email,
                UserName = user.UserName,
                DisplayName = user.DisplayName,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLogin = user.LastLogin,
                Role = user.Role?.RoleName ?? "User",
                ApiKey = user.ApiKey,
                IsAdmin = IsAdmin(),
                IsAstronomer = IsAstronomer(),
                IsResearcher = IsResearcher()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user profile");
            return View("Error");
        }
    }

    public IActionResult Edit()
    {
        return View(new EditProfileViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditProfileViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var userId = GetCurrentUserId();
            var user = await _userService.GetByIdAsync(userId);
            
            if (user == null)
                return NotFound();

            user.UserName = model.UserName;
            await _userService.UpdateUserAsync(user);

            await _loggingService.LogInfoAsync("ProfileUpdated",
                $"Profile updated for user {user.Email}", userId);

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            _logger.LogError(ex, "Error updating profile");
            return View(model);
        }
    }

    public IActionResult ChangePassword()
    {
        return View(new ChangePasswordViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var userId = GetCurrentUserId();
            await _userService.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);

            await _loggingService.LogInfoAsync("PasswordChanged",
                $"Password changed for user {GetCurrentUserEmail()}", userId);

            TempData["SuccessMessage"] = "Password changed successfully!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            _logger.LogError(ex, "Error changing password");
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateApiKey()
    {
        try
        {
            var userId = GetCurrentUserId();
            await _userService.GenerateApiKeyAsync(userId);

            await _loggingService.LogInfoAsync("ApiKeyGenerated",
                $"API key generated for user {GetCurrentUserEmail()}", userId);

            TempData["SuccessMessage"] = "API key generated successfully!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating API key");
            TempData["ErrorMessage"] = "Error generating API key.";
            return RedirectToAction("Index");
        }
    }

    public async Task<IActionResult> Favorites(int page = 1, int pageSize = 10)
    {
        try
        {
            var userId = GetCurrentUserId();
            var favorites = await _userProfileService.GetUserFavoritesAsync(userId);
            var totalFavorites = favorites.Count();
            
            var pagedFavorites = favorites
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(f => new UserFavoriteViewModel
                {
                    FavoriteID = f.FavoriteID,
                    ObjectType = f.ObjectType.ToString(),
                    ObjectID = f.ObjectID,
                    ObjectName = GetObjectName(f.ObjectType.ToString(), f.ObjectID).Result,
                    CreatedAt = f.CreatedAt
                })
                .ToList();

            var model = new UserFavoritesViewModel
            {
                Favorites = pagedFavorites,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalFavorites,
                TotalPages = (int)Math.Ceiling(totalFavorites / (double)pageSize)
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving favorites");
            return View("Error");
        }
    }

    public async Task<IActionResult> History(int page = 1, int pageSize = 20)
    {
        try
        {
            var userId = GetCurrentUserId();
            var history = await _userProfileService.GetUserHistoryAsync(userId);
            var totalHistory = history.Count();
            
            var pagedHistory = history
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(h => new ExplorationHistoryViewModel
                {
                    HistoryID = h.HistoryID,
                    ObjectType = h.ObjectType.ToString(),
                    ObjectID = h.ObjectID,
                    ObjectName = GetObjectName(h.ObjectType.ToString(), h.ObjectID).Result,
                    AccessedAt = h.AccessedAt,
                    DurationSeconds = h.DurationSeconds,
                    SearchCriteria = h.SearchCriteria
                })
                .ToList();

            var model = new UserHistoryViewModel
            {
                History = pagedHistory,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalHistory,
                TotalPages = (int)Math.Ceiling(totalHistory / (double)pageSize)
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving history");
            return View("Error");
        }
    }

    public async Task<IActionResult> SavedSearches()
    {
        try
        {
            var userId = GetCurrentUserId();
            var searches = await _userProfileService.GetUserSavedSearchesAsync(userId);
            
            var model = searches.Select(s => new SavedSearchViewModel
            {
                SavedSearchID = s.SavedSearchID,
                Name = s.Name,
                Criteria = s.Criteria,
                CreatedAt = s.CreatedAt
            }).ToList();

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving saved searches");
            return View("Error");
        }
    }

    public async Task<IActionResult> Discoveries(int page = 1, int pageSize = 10)
    {
        try
        {
            var userId = GetCurrentUserId();
            var discoveries = await _discoveryService.GetUserDiscoveriesAsync(userId);
            var totalDiscoveries = discoveries.Count();
            
            var pagedDiscoveries = discoveries
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new UserDiscoveryViewModel
                {
                    DiscoveryID = d.DiscoveryID,
                    ObjectType = d.ObjectType.ToString(),
                    SuggestedName = d.SuggestedName,
                    State = d.State.ToString(),
                    CreatedAt = d.CreatedAt,
                    Upvotes = d.Upvotes,
                    Downvotes = d.Downvotes,
                    ApprovalRate = d.ApprovalRate
                })
                .ToList();

            var model = new UserDiscoveriesViewModel
            {
                Discoveries = pagedDiscoveries,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalDiscoveries,
                TotalPages = (int)Math.Ceiling(totalDiscoveries / (double)pageSize)
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user discoveries");
            return View("Error");
        }
    }

    public async Task<IActionResult> Articles(int page = 1, int pageSize = 10)
    {
        try
        {
            var userId = GetCurrentUserId();
            var articles = await _contentService.GetArticlesByAuthorAsync(userId);
            var totalArticles = articles.Count();
            
            var pagedArticles = articles
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new UserArticleViewModel
                {
                    ArticleID = a.ArticleID,
                    Title = a.Title,
                    State = a.State.ToString(),
                    CreatedAt = a.CreatedAt,
                    PublishedAt = a.PublishedAt,
                    IsPublished = a.IsPublished
                })
                .ToList();

            var model = new UserArticlesViewModel
            {
                Articles = pagedArticles,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalArticles,
                TotalPages = (int)Math.Ceiling(totalArticles / (double)pageSize)
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user articles");
            return View("Error");
        }
    }

    public async Task<IActionResult> Events(int page = 1, int pageSize = 10)
    {
        try
        {
            var userId = GetCurrentUserId();
            var events = await _contentService.GetAllEventsAsync();
            var userEvents = events.Where(e => e.CreatedByUserID == userId).ToList();
            var totalEvents = userEvents.Count;
            
            var pagedEvents = userEvents
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new UserEventViewModel
                {
                    EventID = e.EventID,
                    Name = e.Name,
                    Type = e.Type.ToString(),
                    EventDate = e.EventDate,
                    CreatedAt = e.CreatedAt,
                    IsUpcoming = e.IsUpcoming
                })
                .ToList();

            var model = new UserEventsViewModel
            {
                Events = pagedEvents,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalEvents,
                TotalPages = (int)Math.Ceiling(totalEvents / (double)pageSize)
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user events");
            return View("Error");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ClearHistory()
    {
        try
        {
            var userId = GetCurrentUserId();
            await _userProfileService.ClearHistoryAsync(userId);

            await _loggingService.LogInfoAsync("HistoryCleared",
                $"History cleared for user {GetCurrentUserEmail()}", userId);

            TempData["SuccessMessage"] = "History cleared successfully!";
            return RedirectToAction("History");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing history");
            TempData["ErrorMessage"] = "Error clearing history.";
            return RedirectToAction("History");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSavedSearch(int id)
    {
        try
        {
            await _userProfileService.DeleteSavedSearchAsync(id);

            await _loggingService.LogInfoAsync("SearchDeleted",
                $"Saved search {id} deleted", GetCurrentUserId());

            TempData["SuccessMessage"] = "Saved search deleted successfully!";
            return RedirectToAction("SavedSearches");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting saved search: {id}");
            TempData["ErrorMessage"] = "Error deleting saved search.";
            return RedirectToAction("SavedSearches");
        }
    }

    private async Task<string> GetObjectName(string objectType, int objectId)
    {
        try
        {
            switch (objectType)
            {
                case "Galaxy":
                    var galaxy = await _astronomicalService.GetGalaxyByIdAsync(objectId);
                    return galaxy?.Name ?? "Unknown Galaxy";
                case "Star":
                    var star = await _astronomicalService.GetStarByIdAsync(objectId);
                    return star?.Name ?? "Unknown Star";
                case "Planet":
                    var planet = await _astronomicalService.GetPlanetByIdAsync(objectId);
                    return planet?.Name ?? "Unknown Planet";
                case "Article":
                    var article = await _contentService.GetArticleByIdAsync(objectId);
                    return article?.Title ?? "Unknown Article";
                case "Event":
                    var eventObj = await _contentService.GetEventByIdAsync(objectId);
                    return eventObj?.Name ?? "Unknown Event";
                case "Discovery":
                    var discovery = await _discoveryService.GetDiscoveryByIdAsync(objectId);
                    return discovery?.SuggestedName ?? "Unknown Discovery";
                default:
                    return "Unknown Object";
            }
        }
        catch
        {
            return "Unknown Object";
        }
    }
}