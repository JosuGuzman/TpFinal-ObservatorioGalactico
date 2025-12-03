namespace Observatorio.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
public class DiscoveriesController : BaseApiController
{
    private readonly IDiscoveryService _discoveryService;
    private readonly ILoggingService _loggingService;

    public DiscoveriesController(IDiscoveryService discoveryService, ILoggingService loggingService)
    {
        _discoveryService = discoveryService;
        _loggingService = loggingService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllDiscoveries(
        [FromQuery] string? state = null, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var discoveries = state != null 
                ? await _discoveryService.GetDiscoveriesByStateAsync(state)
                : await _discoveryService.GetAllDiscoveriesAsync();
            
            var pagedDiscoveries = discoveries
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return SuccessResponse(new
            {
                data = pagedDiscoveries,
                page,
                pageSize,
                totalCount = discoveries.Count(),
                totalPages = (int)Math.Ceiling((double)discoveries.Count() / pageSize)
            });
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving discoveries", ex);
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDiscoveryById(int id)
    {
        try
        {
            var discovery = await _discoveryService.GetDiscoveryByIdAsync(id);
            
            if (discovery == null)
                return NotFoundResponse($"Discovery with ID {id} not found");

            return SuccessResponse(discovery);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving discovery", ex);
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ReportDiscovery([FromBody] ReportDiscoveryRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            
            var discovery = await _discoveryService.CreateDiscoveryAsync(
                userId,
                request.ObjectType,
                request.SuggestedName,
                request.RA,
                request.Dec,
                request.Description,
                request.Attachments);

            await _loggingService.LogInfoAsync("DiscoveryReported", 
                $"Discovery {discovery.DiscoveryID} reported by user {userId}", 
                userId);

            return CreatedResponse($"/api/v1/discoveries/{discovery.DiscoveryID}", discovery);
        }
        catch (Exception ex)
        {
            return BadRequestResponse(ex.Message);
        }
    }

    [HttpPut("{id}/state")]
    [Authorize(Roles = "Astronomer,Admin")]
    public async Task<IActionResult> UpdateDiscoveryState(int id, [FromBody] UpdateStateRequest request)
    {
        try
        {
            var discovery = await _discoveryService.UpdateDiscoveryStateAsync(id, request.State);
            
            await _loggingService.LogInfoAsync("DiscoveryStateUpdated", 
                $"Discovery {id} state updated to {request.State}", 
                GetCurrentUserId());

            return SuccessResponse(discovery, "Discovery state updated successfully");
        }
        catch (Exception ex)
        {
            return BadRequestResponse(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteDiscovery(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var discovery = await _discoveryService.GetDiscoveryByIdAsync(id);
            
            if (discovery == null)
                return NotFoundResponse($"Discovery with ID {id} not found");

            // Solo el creador o un administrador puede eliminar
            if (discovery.ReporterUserID != userId && !IsAdmin())
                return ForbiddenResponse("You can only delete your own discoveries");

            await _discoveryService.DeleteDiscoveryAsync(id);
            
            await _loggingService.LogInfoAsync("DiscoveryDeleted", 
                $"Discovery {id} deleted", 
                userId);

            return SuccessResponse(new { message = $"Discovery {id} deleted successfully" });
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error deleting discovery", ex);
        }
    }

    [HttpPost("{id}/vote")]
    [Authorize]
    public async Task<IActionResult> VoteDiscovery(int id, [FromBody] VoteDiscoveryRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _discoveryService.VoteDiscoveryAsync(id, userId, request.Vote, request.Comment);
            
            return SuccessResponse(new 
            { 
                message = request.Vote ? "Upvote recorded" : "Downvote recorded" 
            });
        }
        catch (Exception ex)
        {
            return BadRequestResponse(ex.Message);
        }
    }

    [HttpDelete("{id}/vote")]
    [Authorize]
    public async Task<IActionResult> RemoveVote(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _discoveryService.RemoveVoteAsync(id, userId);
            
            return SuccessResponse(new { message = "Vote removed successfully" });
        }
        catch (Exception ex)
        {
            return BadRequestResponse(ex.Message);
        }
    }

    [HttpGet("{id}/votes")]
    [AllowAnonymous]
    public async Task<IActionResult> GetVotes(int id)
    {
        try
        {
            var upvotes = await _discoveryService.GetUpvotesCountAsync(id);
            var downvotes = await _discoveryService.GetDownvotesCountAsync(id);
            var approvalRate = await _discoveryService.GetApprovalRateAsync(id);
            
            return SuccessResponse(new
            {
                upvotes,
                downvotes,
                totalVotes = upvotes + downvotes,
                approvalRate
            });
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving votes", ex);
        }
    }

    [HttpGet("user/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetUserDiscoveries(int userId)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            
            // Solo puede ver sus propios descubrimientos a menos que sea admin
            if (userId != currentUserId && !IsAdmin())
                return ForbiddenResponse("You can only view your own discoveries");

            var discoveries = await _discoveryService.GetUserDiscoveriesAsync(userId);
            return SuccessResponse(discoveries);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving user discoveries", ex);
        }
    }

    [HttpGet("pending")]
    [Authorize(Roles = "Astronomer,Admin")]
    public async Task<IActionResult> GetPendingDiscoveries()
    {
        try
        {
            var discoveries = await _discoveryService.GetDiscoveriesByStateAsync("Pendiente");
            return SuccessResponse(discoveries);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving pending discoveries", ex);
        }
    }

    [HttpPost("{id}/validate")]
    [Authorize(Roles = "Astronomer,Admin")]
    public async Task<IActionResult> ValidateDiscovery(int id)
    {
        try
        {
            var astronomerId = GetCurrentUserId();
            var isValidated = await _discoveryService.ValidateDiscoveryAsync(id, astronomerId);
            
            return SuccessResponse(new 
            { 
                message = isValidated ? "Discovery validated successfully" : "Validation failed" 
            });
        }
        catch (Exception ex)
        {
            return BadRequestResponse(ex.Message);
        }
    }

    [HttpPost("{id}/reject")]
    [Authorize(Roles = "Astronomer,Admin")]
    public async Task<IActionResult> RejectDiscovery(int id, [FromBody] RejectRequest request)
    {
        try
        {
            var astronomerId = GetCurrentUserId();
            var isRejected = await _discoveryService.RejectDiscoveryAsync(id, astronomerId, request.Reason);
            
            return SuccessResponse(new 
            { 
                message = isRejected ? "Discovery rejected successfully" : "Rejection failed" 
            });
        }
        catch (Exception ex)
        {
            return BadRequestResponse(ex.Message);
        }
    }

    // Clases auxiliares
    public class UpdateStateRequest
    {
        public string State { get; set; }
    }

    public class RejectRequest
    {
        public string Reason { get; set; }
    }
}