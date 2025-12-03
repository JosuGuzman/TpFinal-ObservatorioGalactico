using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Observatorio.Core.Interfaces;
using Observatorio.Mvc.Models.Astronomical;
using System.Linq.Dynamic.Core;

namespace Observatorio.Mvc.Controllers;

[AllowAnonymous]
public class SearchController : BaseController
{
    private readonly IAstronomicalDataService _astronomicalService;
    private readonly IContentService _contentService;
    private readonly IDiscoveryService _discoveryService;

    public SearchController(
        IAstronomicalDataService astronomicalService,
        IContentService contentService,
        IDiscoveryService discoveryService)
    {
        _astronomicalService = astronomicalService;
        _contentService = contentService;
        _discoveryService = discoveryService;
    }

    public IActionResult Index()
    {
        return View(new SearchViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Index(SearchViewModel model, int page = 1, int pageSize = 20)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var results = new List<SearchResultViewModel>();
            
            // Búsqueda por tipo o búsqueda general
            if (!string.IsNullOrEmpty(model.Query))
            {
                if (string.IsNullOrEmpty(model.Type) || model.Type == "all" || model.Type == "galaxies")
                {
                    var galaxies = await _astronomicalService.SearchGalaxiesAsync(model.Query);
                    results.AddRange(galaxies.Select(g => new SearchResultViewModel
                    {
                        Type = "Galaxy",
                        ID = g.GalaxyID,
                        Name = g.Name,
                        Description = g.Description,
                        Distance = g.DistanceLy,
                        CreatedAt = g.CreatedAt,
                        Relevance = CalculateRelevance(g.Name, g.Description, model.Query)
                    }));
                }

                if (string.IsNullOrEmpty(model.Type) || model.Type == "all" || model.Type == "stars")
                {
                    var stars = await _astronomicalService.SearchStarsAsync(model.Query);
                    results.AddRange(stars.Select(s => new SearchResultViewModel
                    {
                        Type = "Star",
                        ID = s.StarID,
                        Name = s.Name,
                        Description = s.Description,
                        Distance = s.DistanceLy,
                        CreatedAt = s.CreatedAt,
                        Relevance = CalculateRelevance(s.Name, s.Description, model.Query)
                    }));
                }

                if (string.IsNullOrEmpty(model.Type) || model.Type == "all" || model.Type == "planets")
                {
                    var planets = await _astronomicalService.SearchPlanetsAsync(model.Query);
                    results.AddRange(planets.Select(p => new SearchResultViewModel
                    {
                        Type = "Planet",
                        ID = p.PlanetID,
                        Name = p.Name,
                        Description = p.Description,
                        Habitability = p.HabitabilityScore,
                        CreatedAt = p.CreatedAt,
                        Relevance = CalculateRelevance(p.Name, p.Description, model.Query)
                    }));
                }

                if (string.IsNullOrEmpty(model.Type) || model.Type == "all" || model.Type == "articles")
                {
                    var articles = await _contentService.SearchArticlesAsync(model.Query);
                    results.AddRange(articles.Where(a => a.IsPublished).Select(a => new SearchResultViewModel
                    {
                        Type = "Article",
                        ID = a.ArticleID,
                        Name = a.Title,
                        Description = a.Content.Length > 200 ? a.Content.Substring(0, 200) + "..." : a.Content,
                        CreatedAt = a.CreatedAt,
                        Relevance = CalculateRelevance(a.Title, a.Content, model.Query)
                    }));
                }

                if (string.IsNullOrEmpty(model.Type) || model.Type == "all" || model.Type == "events")
                {
                    var events = await _contentService.GetAllEventsAsync();
                    var filteredEvents = events
                        .Where(e => e.Name.Contains(model.Query, StringComparison.OrdinalIgnoreCase) ||
                                   (e.Description?.Contains(model.Query, StringComparison.OrdinalIgnoreCase) ?? false))
                        .Where(e => e.IsUpcoming);
                    
                    results.AddRange(filteredEvents.Select(e => new SearchResultViewModel
                    {
                        Type = "Event",
                        ID = e.EventID,
                        Name = e.Name,
                        Description = e.Description?.Length > 200 ? e.Description.Substring(0, 200) + "..." : e.Description,
                        CreatedAt = e.CreatedAt,
                        Relevance = CalculateRelevance(e.Name, e.Description, model.Query)
                    }));
                }

                if (string.IsNullOrEmpty(model.Type) || model.Type == "all" || model.Type == "discoveries")
                {
                    var discoveries = await _discoveryService.GetAllDiscoveriesAsync();
                    var filteredDiscoveries = discoveries
                        .Where(d => d.SuggestedName.Contains(model.Query, StringComparison.OrdinalIgnoreCase) ||
                                   d.Description.Contains(model.Query, StringComparison.OrdinalIgnoreCase));
                    
                    results.AddRange(filteredDiscoveries.Select(d => new SearchResultViewModel
                    {
                        Type = "Discovery",
                        ID = d.DiscoveryID,
                        Name = d.SuggestedName,
                        Description = d.Description.Length > 200 ? d.Description.Substring(0, 200) + "..." : d.Description,
                        CreatedAt = d.CreatedAt,
                        Relevance = CalculateRelevance(d.SuggestedName, d.Description, model.Query)
                    }));
                }
            }
            else if (!string.IsNullOrEmpty(model.Type))
            {
                // Búsqueda por tipo con filtros específicos
                switch (model.Type)
                {
                    case "galaxies":
                        var galaxies = await _astronomicalService.GetAllGalaxiesAsync();
                        if (model.MinDistance.HasValue)
                            galaxies = galaxies.Where(g => g.DistanceLy >= model.MinDistance.Value);
                        if (model.MaxDistance.HasValue)
                            galaxies = galaxies.Where(g => g.DistanceLy <= model.MaxDistance.Value);
                        if (!string.IsNullOrEmpty(model.GalaxyType))
                            galaxies = galaxies.Where(g => g.Type.ToString() == model.GalaxyType);
                        
                        results.AddRange(galaxies.Select(g => new SearchResultViewModel
                        {
                            Type = "Galaxy",
                            ID = g.GalaxyID,
                            Name = g.Name,
                            Description = g.Description,
                            Distance = g.DistanceLy,
                            CreatedAt = g.CreatedAt
                        }));
                        break;

                    case "stars":
                        var stars = await _astronomicalService.GetAllStarsAsync();
                        if (model.MinDistance.HasValue)
                            stars = stars.Where(s => s.DistanceLy >= model.MinDistance.Value);
                        if (model.MaxDistance.HasValue)
                            stars = stars.Where(s => s.DistanceLy <= model.MaxDistance.Value);
                        if (!string.IsNullOrEmpty(model.SpectralType))
                            stars = stars.Where(s => s.SpectralType.ToString() == model.SpectralType);
                        
                        results.AddRange(stars.Select(s => new SearchResultViewModel
                        {
                            Type = "Star",
                            ID = s.StarID,
                            Name = s.Name,
                            Description = s.Description,
                            Distance = s.DistanceLy,
                            CreatedAt = s.CreatedAt
                        }));
                        break;

                    case "planets":
                        var planets = await _astronomicalService.GetAllPlanetsAsync();
                        if (model.MinHabitability.HasValue)
                            planets = planets.Where(p => p.HabitabilityScore >= model.MinHabitability.Value);
                        if (model.MaxHabitability.HasValue)
                            planets = planets.Where(p => p.HabitabilityScore <= model.MaxHabitability.Value);
                        if (!string.IsNullOrEmpty(model.PlanetType))
                            planets = planets.Where(p => p.PlanetType.ToString() == model.PlanetType);
                        
                        results.AddRange(planets.Select(p => new SearchResultViewModel
                        {
                            Type = "Planet",
                            ID = p.PlanetID,
                            Name = p.Name,
                            Description = p.Description,
                            Habitability = p.HabitabilityScore,
                            CreatedAt = p.CreatedAt
                        }));
                        break;
                }
            }

            // Aplicar ordenamiento
            if (!string.IsNullOrEmpty(model.SortBy))
            {
                try
                {
                    var sortDirection = model.SortDescending ? "desc" : "asc";
                    results = results.AsQueryable().OrderBy($"{model.SortBy} {sortDirection}").ToList();
                }
                catch
                {
                    // Si falla el ordenamiento, ordenar por relevancia por defecto
                    results = results.OrderByDescending(r => r.Relevance).ToList();
                }
            }
            else
            {
                results = results.OrderByDescending(r => r.Relevance).ToList();
            }

            var totalResults = results.Count;
            var pagedResults = results
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            model.Results = new SearchResultsViewModel
            {
                Results = pagedResults,
                Query = model.Query,
                Type = model.Type,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalResults,
                TotalPages = (int)Math.Ceiling(totalResults / (double)pageSize),
                SortBy = model.SortBy,
                SortDescending = model.SortDescending
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing search");
            ModelState.AddModelError("", "An error occurred while performing the search.");
            return View(model);
        }
    }

    public async Task<IActionResult> Nearby(double ra, double dec, double radius = 5.0, int limit = 20)
    {
        try
        {
            var objects = await _astronomicalService.GetNearbyObjectsAsync(ra, dec, radius, limit);
            
            var model = objects.Select(o => new NearbyObjectViewModel
            {
                Type = o.Type,
                ID = o.ID,
                Name = o.Name,
                RA = o.RA,
                Dec = o.Dec,
                Distance = o.Distance
            }).ToList();

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving nearby objects for RA:{ra}, DEC:{dec}, Radius:{radius}");
            return View("Error");
        }
    }

    public async Task<IActionResult> Advanced()
    {
        var model = new AdvancedSearchViewModel
        {
            GalaxyTypes = await GetGalaxyTypes(),
            SpectralTypes = await GetSpectralTypes(),
            PlanetTypes = await GetPlanetTypes(),
            DiscoveryStates = await GetDiscoveryStates()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Advanced(AdvancedSearchViewModel model, int page = 1, int pageSize = 20)
    {
        if (!ModelState.IsValid)
        {
            model.GalaxyTypes = await GetGalaxyTypes();
            model.SpectralTypes = await GetSpectralTypes();
            model.PlanetTypes = await GetPlanetTypes();
            model.DiscoveryStates = await GetDiscoveryStates();
            return View(model);
        }

        try
        {
            var results = new List<SearchResultViewModel>();

            // Aplicar filtros según los criterios
            if (model.SearchGalaxies)
            {
                var galaxies = await _astronomicalService.GetAllGalaxiesAsync();
                
                if (!string.IsNullOrEmpty(model.GalaxyType))
                    galaxies = galaxies.Where(g => g.Type.ToString() == model.GalaxyType);
                
                if (model.MinDistance.HasValue)
                    galaxies = galaxies.Where(g => g.DistanceLy >= model.MinDistance.Value);
                
                if (model.MaxDistance.HasValue)
                    galaxies = galaxies.Where(g => g.DistanceLy <= model.MaxDistance.Value);
                
                if (model.MinMagnitude.HasValue)
                    galaxies = galaxies.Where(g => g.ApparentMagnitude >= model.MinMagnitude.Value);
                
                if (model.MaxMagnitude.HasValue)
                    galaxies = galaxies.Where(g => g.ApparentMagnitude <= model.MaxMagnitude.Value);
                
                results.AddRange(galaxies.Select(g => new SearchResultViewModel
                {
                    Type = "Galaxy",
                    ID = g.GalaxyID,
                    Name = g.Name,
                    Description = g.Description,
                    Distance = g.DistanceLy,
                    CreatedAt = g.CreatedAt
                }));
            }

            if (model.SearchStars)
            {
                var stars = await _astronomicalService.GetAllStarsAsync();
                
                if (!string.IsNullOrEmpty(model.SpectralType))
                    stars = stars.Where(s => s.SpectralType.ToString() == model.SpectralType);
                
                if (model.MinTemp.HasValue)
                    stars = stars.Where(s => s.SurfaceTempK >= model.MinTemp.Value);
                
                if (model.MaxTemp.HasValue)
                    stars = stars.Where(s => s.SurfaceTempK <= model.MaxTemp.Value);
                
                if (model.MinMass.HasValue)
                    stars = stars.Where(s => s.MassSolar >= model.MinMass.Value);
                
                if (model.MaxMass.HasValue)
                    stars = stars.Where(s => s.MassSolar <= model.MaxMass.Value);
                
                results.AddRange(stars.Select(s => new SearchResultViewModel
                {
                    Type = "Star",
                    ID = s.StarID,
                    Name = s.Name,
                    Description = s.Description,
                    Distance = s.DistanceLy,
                    CreatedAt = s.CreatedAt
                }));
            }

            if (model.SearchPlanets)
            {
                var planets = await _astronomicalService.GetAllPlanetsAsync();
                
                if (!string.IsNullOrEmpty(model.PlanetType))
                    planets = planets.Where(p => p.PlanetType.ToString() == model.PlanetType);
                
                if (model.MinHabitability.HasValue)
                    planets = planets.Where(p => p.HabitabilityScore >= model.MinHabitability.Value);
                
                if (model.MaxHabitability.HasValue)
                    planets = planets.Where(p => p.HabitabilityScore <= model.MaxHabitability.Value);
                
                if (model.MinOrbitalDistance.HasValue)
                    planets = planets.Where(p => p.OrbitalDistanceAU >= model.MinOrbitalDistance.Value);
                
                if (model.MaxOrbitalDistance.HasValue)
                    planets = planets.Where(p => p.OrbitalDistanceAU <= model.MaxOrbitalDistance.Value);
                
                results.AddRange(planets.Select(p => new SearchResultViewModel
                {
                    Type = "Planet",
                    ID = p.PlanetID,
                    Name = p.Name,
                    Description = p.Description,
                    Habitability = p.HabitabilityScore,
                    CreatedAt = p.CreatedAt
                }));
            }

            if (model.SearchDiscoveries)
            {
                var discoveries = await _discoveryService.GetAllDiscoveriesAsync();
                
                if (!string.IsNullOrEmpty(model.DiscoveryState))
                    discoveries = discoveries.Where(d => d.State.ToString() == model.DiscoveryState);
                
                if (model.MinVotes.HasValue)
                    discoveries = discoveries.Where(d => d.TotalVotes >= model.MinVotes.Value);
                
                if (model.MinApprovalRate.HasValue)
                    discoveries = discoveries.Where(d => d.ApprovalRate >= model.MinApprovalRate.Value);
                
                results.AddRange(discoveries.Select(d => new SearchResultViewModel
                {
                    Type = "Discovery",
                    ID = d.DiscoveryID,
                    Name = d.SuggestedName,
                    Description = d.Description,
                    CreatedAt = d.CreatedAt
                }));
            }

            // Aplicar ordenamiento
            if (!string.IsNullOrEmpty(model.SortBy))
            {
                try
                {
                    var sortDirection = model.SortDescending ? "desc" : "asc";
                    results = results.AsQueryable().OrderBy($"{model.SortBy} {sortDirection}").ToList();
                }
                catch
                {
                    results = results.OrderByDescending(r => r.CreatedAt).ToList();
                }
            }
            else
            {
                results = results.OrderByDescending(r => r.CreatedAt).ToList();
            }

            var totalResults = results.Count;
            var pagedResults = results
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            model.Results = new SearchResultsViewModel
            {
                Results = pagedResults,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalResults,
                TotalPages = (int)Math.Ceiling(totalResults / (double)pageSize)
            };

            // Restaurar las listas para el dropdown
            model.GalaxyTypes = await GetGalaxyTypes();
            model.SpectralTypes = await GetSpectralTypes();
            model.PlanetTypes = await GetPlanetTypes();
            model.DiscoveryStates = await GetDiscoveryStates();

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing advanced search");
            ModelState.AddModelError("", "An error occurred while performing the search.");
            
            // Restaurar las listas
            model.GalaxyTypes = await GetGalaxyTypes();
            model.SpectralTypes = await GetSpectralTypes();
            model.PlanetTypes = await GetPlanetTypes();
            model.DiscoveryStates = await GetDiscoveryStates();
            
            return View(model);
        }
    }

    private float CalculateRelevance(string name, string description, string query)
    {
        if (string.IsNullOrEmpty(query))
            return 0;

        var queryTerms = query.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var nameLower = name?.ToLower() ?? "";
        var descriptionLower = description?.ToLower() ?? "";

        float relevance = 0;
        
        foreach (var term in queryTerms)
        {
            if (nameLower.Contains(term))
                relevance += 2.0f; // Más peso para coincidencias en el nombre
            
            if (descriptionLower.Contains(term))
                relevance += 1.0f; // Menos peso para coincidencias en la descripción
        }

        return relevance;
    }

    private async Task<List<string>> GetGalaxyTypes()
    {
        return new List<string>
        {
            "Espiral",
            "Eliptica",
            "Irregular",
            "Lenticular"
        };
    }

    private async Task<List<string>> GetSpectralTypes()
    {
        return new List<string>
        {
            "O", "B", "A", "F", "G", "K", "M", "Unknown"
        };
    }

    private async Task<List<string>> GetPlanetTypes()
    {
        return new List<string>
        {
            "Terrestre",
            "GiganteGas",
            "EnanaHielo",
            "Otro"
        };
    }

    private async Task<List<string>> GetDiscoveryStates()
    {
        return new List<string>
        {
            "Pendiente",
            "ValidacionComunitaria",
            "RevisadoAstronomo",
            "Aprobado",
            "Rechazado"
        };
    }
}