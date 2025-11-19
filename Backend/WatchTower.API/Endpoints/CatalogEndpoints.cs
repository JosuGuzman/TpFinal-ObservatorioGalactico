using WatchTower.API.Models.DTOs;
using WatchTower.API.Repositories;

namespace WatchTower.API.Endpoints;

public static class CatalogEndpoints
{
    public static void MapCatalogEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/catalog");
        
        group.MapGet("/bodies", async (
            string? searchTerm,
            string? bodyType,
            decimal? maxDistance,
            decimal? minMagnitude,
            ICelestialBodyRepository repo) =>
        {
            var request = new CelestialBodySearchRequest
            {
                SearchTerm = searchTerm,
                BodyType = bodyType,
                MaxDistance = maxDistance,
                MinMagnitude = minMagnitude
            };
            
            var bodies = await repo.SearchCelestialBodiesAsync(request);
            return Results.Ok(bodies);
        });
        
        group.MapGet("/bodies/{id}", async (int id, ICelestialBodyRepository repo) =>
        {
            var body = await repo.GetByIdAsync(id);
            return body != null ? Results.Ok(body) : Results.NotFound();
        });
        
        group.MapGet("/bodies/recent/{count}", async (int count, ICelestialBodyRepository repo) =>
        {
            var bodies = await repo.GetRecentBodiesAsync(count);
            return Results.Ok(bodies);
        });
    }
}