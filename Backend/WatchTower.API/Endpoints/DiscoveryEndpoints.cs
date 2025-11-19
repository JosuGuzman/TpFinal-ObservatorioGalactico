using WatchTower.API.Models.DTOs;
using WatchTower.API.Repositories;
using System.Security.Claims;

namespace WatchTower.API.Endpoints;

public static class DiscoveryEndpoints
{
    public static void MapDiscoveryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/discoveries").RequireAuthorization();
        
        group.MapGet("/", async (string? search, string? status, IDiscoveryRepository repo) =>
        {
            var discoveries = await repo.GetDiscoveriesAsync(search, status);
            return Results.Ok(discoveries);
        });
        
        group.MapGet("/{id}", async (int id, IDiscoveryRepository repo) =>
        {
            var discovery = await repo.GetByIdAsync(id);
            return discovery != null ? Results.Ok(discovery) : Results.NotFound();
        });
        
        group.MapPost("/", async (DiscoveryCreateRequest request, IDiscoveryRepository repo, HttpContext context) =>
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            var discovery = new Models.Entities.Discovery
            {
                Title = request.Title,
                Description = request.Description,
                Coordinates = request.Coordinates,
                CelestialBodyId = request.CelestialBodyId,
                ReportedBy = userId,
                Status = "Pending"
            };
            
            var discoveryId = await repo.AddDiscoveryAsync(discovery);
            return Results.Created($"/discoveries/{discoveryId}", new { id = discoveryId });
        });
        
        group.MapPost("/{id}/vote", async (int id, string voteType, IDiscoveryRepository repo, HttpContext context) =>
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            if (voteType != "Up" && voteType != "Down")
                return Results.BadRequest(new { error = "Vote type must be 'Up' or 'Down'" });
            
            var success = await repo.AddVoteAsync(id, userId, voteType);
            return success ? Results.Ok(new { message = "Vote recorded" }) 
                            : Results.BadRequest(new { error = "Failed to record vote" });
        });
    }
}