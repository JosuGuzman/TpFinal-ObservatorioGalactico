using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Observatorio.Core.Entities.Content;
using Observatorio.Core.Enums;
using Observatorio.Core.Interfaces;
using Observatorio.Mvc.Models.Discovery;

namespace Observatorio.Mvc.Controllers;

public class DiscoveryController : BaseController
{
    private readonly IDiscoveryService _discoveryService;
    private readonly ILogger<DiscoveryController> _logger;

    public DiscoveryController(
        IDiscoveryService discoveryService,
        ILogger<DiscoveryController> logger)
    {
        _discoveryService = discoveryService;
        _logger = logger;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(
        string state = null,
        string type = null,
        int page = 1,
        int pageSize = 20,
        string sortBy = "created",
        bool sortDesc = true)
    {
        try
        {
            IEnumerable<Discovery> discoveries;

            if (!string.IsNullOrEmpty(state))
                discoveries = await _discoveryService.GetDiscoveriesByStateAsync(state);
            else
                discoveries = await _discoveryService.GetAllDiscoveriesAsync();

            if (!string.IsNullOrEmpty(type) && Enum.TryParse<ObjectType>(type, out var objectType))
                discoveries = discoveries.Where(d => d.ObjectType == objectType);

            // Ordenar
            discoveries = sortBy.ToLower() switch
            {
                "votes" => sortDesc ? discoveries.OrderByDescending(d => d.TotalVotes) : discoveries.OrderBy(d => d.TotalVotes),
                "approval" => sortDesc ? discoveries.OrderByDescending(d => d.ApprovalRate) : discoveries.OrderBy(d => d.ApprovalRate),
                "name" => sortDesc ? discoveries.OrderByDescending(d => d.SuggestedName) : discoveries.OrderBy(d => d.SuggestedName),
                _ => sortDesc ? discoveries.OrderByDescending(d => d.CreatedAt) : discoveries.OrderBy(d => d.CreatedAt)
            };

            var totalCount = discoveries.Count();
            var pagedDiscoveries = discoveries
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new DiscoveryListViewModel
            {
                Discoveries = pagedDiscoveries.Select(d => new DiscoveryViewModel
                {
                    DiscoveryID = d.DiscoveryID,
                    ReporterUserID = d.ReporterUserID,
                    ReporterName = d.Reporter?.UserName ?? "Usuario",
                    ObjectType = d.ObjectType,
                    SuggestedName = d.SuggestedName,
                    Description = d.Description,
                    State = d.State,
                    Upvotes = d.Upvotes,
                    Downvotes = d.Downvotes,
                    ApprovalRate = d.ApprovalRate,
                    CreatedAt = d.CreatedAt,
                    CanEdit = IsAdmin() || IsAstronomer() || d.ReporterUserID == GetCurrentUserId()
                }).ToList(),
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                FilterState = state,
                FilterType = type,
                SortBy = sortBy,
                SortDescending = sortDesc,
                AvailableStates = Enum.GetNames(typeof(DiscoveryState)).ToList(),
                AvailableObjectTypes = Enum.GetNames(typeof(ObjectType)).Where(ot => ot != "Otro").ToList()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading discoveries");
            AddErrorMessage("Error al cargar los descubrimientos");
            return View(new DiscoveryListViewModel());
        }
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var discovery = await _discoveryService.GetDiscoveryByIdAsync(id);
            
            if (discovery == null)
            {
                AddErrorMessage("Descubrimiento no encontrado");
                return RedirectToAction("Index");
            }

            var userId = GetCurrentUserId();
            var hasVoted = userId > 0 ? await _discoveryService.HasUserVotedAsync(id, userId) : false;
            var userVote = hasVoted ? await _discoveryService.GetUserVoteAsync(id, userId) : false;

            var model = new DiscoveryDetailViewModel
            {
                DiscoveryID = discovery.DiscoveryID,
                ReporterUserID = discovery.ReporterUserID,
                ReporterName = discovery.Reporter?.UserName ?? "Usuario",
                ObjectType = discovery.ObjectType,
                SuggestedName = discovery.SuggestedName,
                RA = discovery.RA,
                Dec = discovery.Dec,
                Description = discovery.Description,
                Attachments = discovery.Attachments,
                State = discovery.State,
                Upvotes = discovery.Upvotes,
                Downvotes = discovery.Downvotes,
                ApprovalRate = discovery.ApprovalRate,
                CreatedAt = discovery.CreatedAt,
                UpdatedAt = discovery.UpdatedAt,
                HasVoted = hasVoted,
                UserVote = userVote,
                CanEdit = IsAdmin() || IsAstronomer() || discovery.ReporterUserID == userId,
                CanValidate = IsAdmin() || IsAstronomer(),
                CanDelete = IsAdmin() || discovery.ReporterUserID == userId
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading discovery details");
            AddErrorMessage("Error al cargar los detalles del descubrimiento");
            return RedirectToAction("Index");
        }
    }

    [HttpGet]
    [Authorize(Roles = "Researcher,Astronomer,Admin")]
    public IActionResult Report()
    {
        var model = new ReportDiscoveryViewModel
        {
            AvailableObjectTypes = Enum.GetNames(typeof(ObjectType))
                .Where(ot => ot != "Otro" && ot != "Artículo" && ot != "Evento")
                .ToList()
        };
        
        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Researcher,Astronomer,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Report(ReportDiscoveryViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableObjectTypes = Enum.GetNames(typeof(ObjectType))
                .Where(ot => ot != "Otro" && ot != "Artículo" && ot != "Evento")
                .ToList();
            return View(model);
        }

        try
        {
            var userId = GetCurrentUserId();
            
            // Validar coordenadas
            if (model.RA < 0 || model.RA > 360 || model.Dec < -90 || model.Dec > 90)
            {
                ModelState.AddModelError("", "Coordenadas inválidas");
                model.AvailableObjectTypes = Enum.GetNames(typeof(ObjectType))
                    .Where(ot => ot != "Otro" && ot != "Artículo" && ot != "Evento")
                    .ToList();
                return View(model);
            }

            var discovery = await _discoveryService.CreateDiscoveryAsync(
                userId,
                model.ObjectType.ToString(),
                model.SuggestedName,
                model.RA,
                model.Dec,
                model.Description,
                model.Attachments);

            AddSuccessMessage("Descubrimiento reportado exitosamente. Estará pendiente de revisión.");
            return RedirectToAction("Details", new { id = discovery.DiscoveryID });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reporting discovery");
            ModelState.AddModelError("", "Error al reportar el descubrimiento: " + ex.Message);
            model.AvailableObjectTypes = Enum.GetNames(typeof(ObjectType))
                .Where(ot => ot != "Otro" && ot != "Artículo" && ot != "Evento")
                .ToList();
            return View(model);
        }
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Vote(int id, bool vote)
    {
        try
        {
            var userId = GetCurrentUserId();
            
            if (await _discoveryService.HasUserVotedAsync(id, userId))
            {
                await _discoveryService.RemoveVoteAsync(id, userId);
            }
            
            await _discoveryService.VoteDiscoveryAsync(id, userId, vote, null);
            
            AddSuccessMessage(vote ? "Voto positivo registrado" : "Voto negativo registrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error voting");
            AddErrorMessage("Error al registrar el voto: " + ex.Message);
        }
        
        return RedirectToAction("Details", new { id });
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveVote(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _discoveryService.RemoveVoteAsync(id, userId);
            
            AddSuccessMessage("Voto eliminado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing vote");
            AddErrorMessage("Error al eliminar el voto: " + ex.Message);
        }
        
        return RedirectToAction("Details", new { id });
    }

    [HttpGet]
    [Authorize(Roles = "Astronomer,Admin")]
    public async Task<IActionResult> Pending()
    {
        try
        {
            var discoveries = await _discoveryService.GetDiscoveriesByStateAsync("Pendiente");
            
            var model = new DiscoveryListViewModel
            {
                Discoveries = discoveries.Select(d => new DiscoveryViewModel
                {
                    DiscoveryID = d.DiscoveryID,
                    ReporterName = d.Reporter?.UserName ?? "Usuario",
                    ObjectType = d.ObjectType,
                    SuggestedName = d.SuggestedName,
                    Description = d.Description,
                    CreatedAt = d.CreatedAt,
                    Upvotes = d.Upvotes,
                    Downvotes = d.Downvotes,
                    ApprovalRate = d.ApprovalRate
                }).ToList(),
                FilterState = "Pendiente"
            };

            return View("Index", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading pending discoveries");
            AddErrorMessage("Error al cargar los descubrimientos pendientes");
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Astronomer,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Validate(int id)
    {
        try
        {
            var astronomerId = GetCurrentUserId();
            await _discoveryService.ValidateDiscoveryAsync(id, astronomerId);
            
            AddSuccessMessage("Descubrimiento validado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating discovery");
            AddErrorMessage("Error al validar el descubrimiento: " + ex.Message);
        }
        
        return RedirectToAction("Details", new { id });
    }

    [HttpPost]
    [Authorize(Roles = "Astronomer,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id, string reason)
    {
        try
        {
            var astronomerId = GetCurrentUserId();
            await _discoveryService.RejectDiscoveryAsync(id, astronomerId, reason);
            
            AddSuccessMessage("Descubrimiento rechazado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting discovery");
            AddErrorMessage("Error al rechazar el descubrimiento: " + ex.Message);
        }
        
        return RedirectToAction("Details", new { id });
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> MyDiscoveries()
    {
        try
        {
            var userId = GetCurrentUserId();
            var discoveries = await _discoveryService.GetUserDiscoveriesAsync(userId);
            
            var model = new DiscoveryListViewModel
            {
                Discoveries = discoveries.Select(d => new DiscoveryViewModel
                {
                    DiscoveryID = d.DiscoveryID,
                    ObjectType = d.ObjectType,
                    SuggestedName = d.SuggestedName,
                    Description = d.Description,
                    State = d.State,
                    CreatedAt = d.CreatedAt,
                    Upvotes = d.Upvotes,
                    Downvotes = d.Downvotes,
                    ApprovalRate = d.ApprovalRate,
                    CanEdit = true
                }).ToList()
            };

            return View("Index", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user discoveries");
            AddErrorMessage("Error al cargar tus descubrimientos");
            return RedirectToAction("Index");
        }
    }

    [HttpGet]
    [Authorize(Roles = "Astronomer,Admin")]
    public async Task<IActionResult> ValidateDiscovery(int id)
    {
        try
        {
            var discovery = await _discoveryService.GetDiscoveryByIdAsync(id);
            
            if (discovery == null)
            {
                AddErrorMessage("Descubrimiento no encontrado");
                return RedirectToAction("Index");
            }

            if (discovery.State != DiscoveryState.RevisadoAstronomo)
            {
                AddErrorMessage("Este descubrimiento no está listo para validación");
                return RedirectToAction("Details", new { id });
            }

            return View(new ValidateDiscoveryViewModel
            {
                DiscoveryID = discovery.DiscoveryID,
                SuggestedName = discovery.SuggestedName,
                ObjectType = discovery.ObjectType
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading discovery for validation");
            AddErrorMessage("Error al cargar el descubrimiento para validación");
            return RedirectToAction("Details", new { id });
        }
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var discovery = await _discoveryService.GetDiscoveryByIdAsync(id);
            
            if (discovery == null)
            {
                AddErrorMessage("Descubrimiento no encontrado");
                return RedirectToAction("Index");
            }

            // Solo el creador o un administrador puede eliminar
            if (discovery.ReporterUserID != userId && !IsAdmin())
            {
                AddErrorMessage("No tienes permiso para eliminar este descubrimiento");
                return RedirectToAction("Details", new { id });
            }

            await _discoveryService.DeleteDiscoveryAsync(id);
            
            AddSuccessMessage("Descubrimiento eliminado exitosamente");
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting discovery");
            AddErrorMessage("Error al eliminar el descubrimiento: " + ex.Message);
            return RedirectToAction("Details", new { id });
        }
    }
}