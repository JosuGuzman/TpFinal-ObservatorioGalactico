using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Observatorio.Core.Entities.Content;
using Observatorio.Core.Interfaces;
using Observatorio.Mvc.Models.Event;
using System.Security.Claims;

namespace Observatorio.Mvc.Controllers;

[Authorize]
public class EventController : BaseController
{
    private readonly IContentService _contentService;
    private readonly ILoggingService _loggingService;

    public EventController(IContentService contentService, ILoggingService loggingService)
    {
        _contentService = contentService;
        _loggingService = loggingService;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(string type = null, int page = 1, int pageSize = 10)
    {
        try
        {
            IEnumerable<Event> events;
            if (!string.IsNullOrEmpty(type))
                events = await _contentService.GetEventsByTypeAsync(type);
            else
                events = await _contentService.GetAllEventsAsync();

            var upcomingEvents = events.Where(e => e.IsUpcoming).ToList();
            var totalEvents = upcomingEvents.Count;
            
            var pagedEvents = upcomingEvents
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new EventViewModel
                {
                    EventID = e.EventID,
                    Name = e.Name,
                    Type = e.Type.ToString(),
                    EventDate = e.EventDate,
                    Description = e.Description?.Length > 150 ? e.Description.Substring(0, 150) + "..." : e.Description,
                    DurationMinutes = e.DurationMinutes,
                    CreatedByName = e.CreatedBy?.DisplayName ?? "Unknown",
                    CreatedAt = e.CreatedAt,
                    IsUpcoming = e.IsUpcoming,
                    TimeUntilEvent = e.TimeUntilEvent?.ToString(@"dd\.hh\:mm\:ss") ?? "N/A"
                })
                .ToList();

            var model = new EventIndexViewModel
            {
                Events = pagedEvents,
                EventTypes = await GetEventTypes(),
                SelectedType = type,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalEvents,
                TotalPages = (int)Math.Ceiling(totalEvents / (double)pageSize)
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving events");
            return View("Error");
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var eventObj = await _contentService.GetEventByIdAsync(id);
            if (eventObj == null)
                return NotFound();

            var model = new EventViewModel
            {
                EventID = eventObj.EventID,
                Name = eventObj.Name,
                Type = eventObj.Type.ToString(),
                EventDate = eventObj.EventDate,
                Description = eventObj.Description,
                Visibility = eventObj.Visibility,
                Coordinates = eventObj.Coordinates,
                DurationMinutes = eventObj.DurationMinutes,
                Resources = eventObj.Resources,
                CreatedByUserID = eventObj.CreatedByUserID,
                CreatedByName = eventObj.CreatedBy?.DisplayName ?? "Unknown",
                CreatedAt = eventObj.CreatedAt,
                UpdatedAt = eventObj.UpdatedAt,
                IsUpcoming = eventObj.IsUpcoming,
                TimeUntilEvent = eventObj.TimeUntilEvent?.ToString(@"dd\.hh\:mm\:ss") ?? "N/A",
                CanEdit = eventObj.CreatedByUserID == GetCurrentUserId() || IsAdmin()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving event: {id}");
            return View("Error");
        }
    }

    [Authorize(Roles = "Researcher,Astronomer,Admin")]
    public IActionResult Create()
    {
        var model = new CreateEventViewModel
        {
            EventDate = DateTime.Now.AddDays(7)
        };
        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Researcher,Astronomer,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateEventViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var creatorId = GetCurrentUserId();
            var eventObj = await _contentService.CreateEventAsync(
                model.Name,
                model.Type,
                model.EventDate,
                model.Description,
                creatorId,
                model.Visibility,
                model.Coordinates,
                model.DurationMinutes,
                model.Resources);

            await _loggingService.LogInfoAsync("EventCreated",
                $"Event '{model.Name}' created by user {creatorId}", creatorId);

            TempData["SuccessMessage"] = "Event created successfully!";
            return RedirectToAction("Details", new { id = eventObj.EventID });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            _logger.LogError(ex, "Error creating event");
            return View(model);
        }
    }

    [Authorize(Roles = "Researcher,Astronomer,Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var eventObj = await _contentService.GetEventByIdAsync(id);
            if (eventObj == null)
                return NotFound();

            var currentUserId = GetCurrentUserId();
            if (eventObj.CreatedByUserID != currentUserId && !IsAdmin())
                return Forbid();

            var model = new EditEventViewModel
            {
                EventID = eventObj.EventID,
                Name = eventObj.Name,
                Type = eventObj.Type.ToString(),
                EventDate = eventObj.EventDate,
                Description = eventObj.Description,
                Visibility = eventObj.Visibility,
                Coordinates = eventObj.Coordinates,
                DurationMinutes = eventObj.DurationMinutes,
                Resources = eventObj.Resources
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving event for edit: {id}");
            return View("Error");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Researcher,Astronomer,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditEventViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var eventObj = await _contentService.GetEventByIdAsync(id);
            if (eventObj == null)
                return NotFound();

            var currentUserId = GetCurrentUserId();
            if (eventObj.CreatedByUserID != currentUserId && !IsAdmin())
                return Forbid();

            await _contentService.UpdateEventAsync(
                id,
                model.Name,
                model.Type,
                model.EventDate,
                model.Description);

            TempData["SuccessMessage"] = "Event updated successfully!";
            return RedirectToAction("Details", new { id });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            _logger.LogError(ex, $"Error updating event: {id}");
            return View(model);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Researcher,Astronomer,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var eventObj = await _contentService.GetEventByIdAsync(id);
            if (eventObj == null)
                return NotFound();

            var currentUserId = GetCurrentUserId();
            if (eventObj.CreatedByUserID != currentUserId && !IsAdmin())
                return Forbid();

            await _contentService.DeleteEventAsync(id);
            
            await _loggingService.LogInfoAsync("EventDeleted",
                $"Event {id} deleted", currentUserId);

            TempData["SuccessMessage"] = "Event deleted successfully!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting event: {id}");
            TempData["ErrorMessage"] = "Error deleting event.";
            return RedirectToAction("Details", new { id });
        }
    }

    [AllowAnonymous]
    public async Task<IActionResult> Upcoming(int limit = 10)
    {
        try
        {
            var events = await _contentService.GetUpcomingEventsAsync(limit);
            
            var model = events.Select(e => new EventViewModel
            {
                EventID = e.EventID,
                Name = e.Name,
                Type = e.Type.ToString(),
                EventDate = e.EventDate,
                Description = e.Description?.Length > 100 ? e.Description.Substring(0, 100) + "..." : e.Description,
                DurationMinutes = e.DurationMinutes,
                CreatedByName = e.CreatedBy?.DisplayName ?? "Unknown",
                IsUpcoming = e.IsUpcoming,
                TimeUntilEvent = e.TimeUntilEvent?.ToString(@"dd\.hh\:mm\:ss") ?? "N/A"
            }).ToList();

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving upcoming events");
            return View("Error");
        }
    }

    [AllowAnonymous]
    public async Task<IActionResult> Past(int page = 1, int pageSize = 10)
    {
        try
        {
            var events = await _contentService.GetAllEventsAsync();
            var pastEvents = events.Where(e => e.IsPast).ToList();
            var totalEvents = pastEvents.Count;
            
            var pagedEvents = pastEvents
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new EventViewModel
                {
                    EventID = e.EventID,
                    Name = e.Name,
                    Type = e.Type.ToString(),
                    EventDate = e.EventDate,
                    Description = e.Description?.Length > 150 ? e.Description.Substring(0, 150) + "..." : e.Description,
                    DurationMinutes = e.DurationMinutes,
                    CreatedByName = e.CreatedBy?.DisplayName ?? "Unknown",
                    CreatedAt = e.CreatedAt,
                    IsUpcoming = e.IsUpcoming
                })
                .ToList();

            var model = new EventPastViewModel
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
            _logger.LogError(ex, "Error retrieving past events");
            return View("Error");
        }
    }

    [AllowAnonymous]
    public async Task<IActionResult> Calendar(int year = 0, int month = 0)
    {
        try
        {
            var now = DateTime.Now;
            year = year == 0 ? now.Year : year;
            month = month == 0 ? now.Month : month;

            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var events = await _contentService.GetEventsByDateRangeAsync(startDate, endDate);
            
            var model = new EventCalendarViewModel
            {
                Year = year,
                Month = month,
                MonthName = startDate.ToString("MMMM"),
                Events = events.Select(e => new CalendarEventViewModel
                {
                    EventID = e.EventID,
                    Name = e.Name,
                    Type = e.Type.ToString(),
                    EventDate = e.EventDate,
                    Description = e.Description,
                    IsUpcoming = e.IsUpcoming
                }).ToList()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving calendar events");
            return View("Error");
        }
    }

    [AllowAnonymous]
    public async Task<IActionResult> Search(string query, int page = 1, int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(query))
            return RedirectToAction("Index");

        try
        {
            var events = await _contentService.GetAllEventsAsync();
            var filteredEvents = events
                .Where(e => e.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                           (e.Description?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false))
                .Where(e => e.IsUpcoming)
                .ToList();

            var totalEvents = filteredEvents.Count;
            
            var pagedEvents = filteredEvents
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new EventViewModel
                {
                    EventID = e.EventID,
                    Name = e.Name,
                    Type = e.Type.ToString(),
                    EventDate = e.EventDate,
                    Description = e.Description?.Length > 150 ? e.Description.Substring(0, 150) + "..." : e.Description,
                    DurationMinutes = e.DurationMinutes,
                    CreatedByName = e.CreatedBy?.DisplayName ?? "Unknown",
                    CreatedAt = e.CreatedAt,
                    IsUpcoming = e.IsUpcoming,
                    TimeUntilEvent = e.TimeUntilEvent?.ToString(@"dd\.hh\:mm\:ss") ?? "N/A"
                })
                .ToList();

            var model = new EventSearchViewModel
            {
                Events = pagedEvents,
                Query = query,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalEvents,
                TotalPages = (int)Math.Ceiling(totalEvents / (double)pageSize)
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error searching events: {query}");
            return View("Error");
        }
    }

    private async Task<List<string>> GetEventTypes()
    {
        // Retorna los tipos de eventos disponibles
        return new List<string>
        {
            "Eclipse",
            "LluviaMeteoros",
            "Conjunction",
            "Otro"
        };
    }
}