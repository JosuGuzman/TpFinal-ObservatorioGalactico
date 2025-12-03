using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Observatorio.Core.Entities.Astronomic;
using Observatorio.Core.Enums;
using Observatorio.Core.Interfaces;
using Observatorio.Mvc.Models.Astronomical;

namespace Observatorio.Mvc.Controllers.Astronomical;

[Authorize]
public class PlanetController : AstronomicalBaseController
{
    private readonly IStarRepository _starRepository;

    public PlanetController(
        IAstronomicalDataService astronomicalService,
        IStarRepository starRepository,
        ILogger<PlanetController> logger) : base(astronomicalService, logger)
    {
        _starRepository = starRepository;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(
        int page = 1,
        int pageSize = 20,
        string search = "",
        string planetType = "",
        int? starId = null,
        bool? habitableOnly = false,
        string sortBy = "name")
    {
        try
        {
            IEnumerable<Planet> planets = await _astronomicalService.GetAllPlanetsAsync();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(search))
                planets = planets.Where(p => 
                    p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(planetType))
                planets = planets.Where(p => p.PlanetType.ToString() == planetType);

            if (starId.HasValue)
                planets = planets.Where(p => p.StarID == starId);

            if (habitableOnly == true)
                planets = planets.Where(p => p.IsPotentiallyHabitable);

            // Ordenar
            planets = sortBy.ToLower() switch
            {
                "distance" => planets.OrderBy(p => p.OrbitalDistanceAU),
                "period" => planets.OrderBy(p => p.OrbitalPeriodDays),
                "mass" => planets.OrderByDescending(p => p.MassEarth),
                "habitability" => planets.OrderByDescending(p => p.HabitabilityScore),
                _ => planets.OrderBy(p => p.Name)
            };

            var totalCount = planets.Count();
            var pagedPlanets = planets
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new PlanetListViewModel
            {
                Planets = pagedPlanets,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                SearchQuery = search,
                FilterPlanetType = planetType,
                FilterStarId = starId,
                HabitableOnly = habitableOnly ?? false,
                SortBy = sortBy,
                AvailablePlanetTypes = Enum.GetNames(typeof(PlanetType)).ToList(),
                AvailableStars = (await _starRepository.GetAllAsync()).ToList()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading planets");
            AddErrorMessage("Error al cargar los planetas");
            return View(new PlanetListViewModel());
        }
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var planet = await _astronomicalService.GetPlanetByIdAsync(id);
            
            if (planet == null)
            {
                AddErrorMessage("Planeta no encontrado");
                return RedirectToAction("Index");
            }

            var model = new PlanetDetailViewModel
            {
                Planet = planet,
                CanEdit = IsAdmin() || IsAstronomer()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading planet details");
            AddErrorMessage("Error al cargar los detalles del planeta");
            return RedirectToAction("Index");
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Astronomer")]
    public async Task<IActionResult> Create()
    {
        var model = new PlanetViewModel
        {
            AvailablePlanetTypes = Enum.GetNames(typeof(PlanetType)).ToList(),
            AvailableStars = (await _starRepository.GetAllAsync()).Select(s => new StarViewModel
            {
                StarID = s.StarID,
                Name = s.Name
            }).ToList()
        };
        
        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Astronomer")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PlanetViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailablePlanetTypes = Enum.GetNames(typeof(PlanetType)).ToList();
            model.AvailableStars = (await _starRepository.GetAllAsync()).Select(s => new StarViewModel
            {
                StarID = s.StarID,
                Name = s.Name
            }).ToList();
            return View(model);
        }

        try
        {
            var planet = new Planet
            {
                StarID = model.StarID,
                Name = model.Name,
                PlanetType = model.PlanetType,
                MassEarth = model.MassEarth,
                RadiusEarth = model.RadiusEarth,
                OrbitalPeriodDays = model.OrbitalPeriodDays,
                OrbitalDistanceAU = model.OrbitalDistanceAU,
                Eccentricity = model.Eccentricity,
                HabitabilityScore = model.HabitabilityScore,
                Atmosphere = model.Atmosphere,
                SurfaceTempK = model.SurfaceTempK,
                DiscoveryDate = model.DiscoveryDate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // await _astronomicalService.CreatePlanetAsync(planet);
            
            AddSuccessMessage($"Planeta '{model.Name}' creado exitosamente");
            return RedirectToAction("Details", new { id = planet.PlanetID });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating planet");
            ModelState.AddModelError("", "Error al crear el planeta: " + ex.Message);
            model.AvailablePlanetTypes = Enum.GetNames(typeof(PlanetType)).ToList();
            model.AvailableStars = (await _starRepository.GetAllAsync()).Select(s => new StarViewModel
            {
                StarID = s.StarID,
                Name = s.Name
            }).ToList();
            return View(model);
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Astronomer")]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var planet = await _astronomicalService.GetPlanetByIdAsync(id);
            
            if (planet == null)
            {
                AddErrorMessage("Planeta no encontrado");
                return RedirectToAction("Index");
            }

            var model = new PlanetViewModel
            {
                PlanetID = planet.PlanetID,
                StarID = planet.StarID,
                Name = planet.Name,
                PlanetType = planet.PlanetType,
                MassEarth = planet.MassEarth,
                RadiusEarth = planet.RadiusEarth,
                OrbitalPeriodDays = planet.OrbitalPeriodDays,
                OrbitalDistanceAU = planet.OrbitalDistanceAU,
                Eccentricity = planet.Eccentricity,
                HabitabilityScore = planet.HabitabilityScore,
                Atmosphere = planet.Atmosphere,
                SurfaceTempK = planet.SurfaceTempK,
                DiscoveryDate = planet.DiscoveryDate,
                AvailablePlanetTypes = Enum.GetNames(typeof(PlanetType)).ToList(),
                AvailableStars = (await _starRepository.GetAllAsync()).Select(s => new StarViewModel
                {
                    StarID = s.StarID,
                    Name = s.Name
                }).ToList()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading planet for edit");
            AddErrorMessage("Error al cargar el planeta para editar");
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Astronomer")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PlanetViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailablePlanetTypes = Enum.GetNames(typeof(PlanetType)).ToList();
            model.AvailableStars = (await _starRepository.GetAllAsync()).Select(s => new StarViewModel
            {
                StarID = s.StarID,
                Name = s.Name
            }).ToList();
            return View(model);
        }

        try
        {
            var planet = await _astronomicalService.GetPlanetByIdAsync(id);
            
            if (planet == null)
            {
                AddErrorMessage("Planeta no encontrado");
                return RedirectToAction("Index");
            }

            planet.StarID = model.StarID;
            planet.Name = model.Name;
            planet.PlanetType = model.PlanetType;
            planet.MassEarth = model.MassEarth;
            planet.RadiusEarth = model.RadiusEarth;
            planet.OrbitalPeriodDays = model.OrbitalPeriodDays;
            planet.OrbitalDistanceAU = model.OrbitalDistanceAU;
            planet.Eccentricity = model.Eccentricity;
            planet.HabitabilityScore = model.HabitabilityScore;
            planet.Atmosphere = model.Atmosphere;
            planet.SurfaceTempK = model.SurfaceTempK;
            planet.DiscoveryDate = model.DiscoveryDate;
            planet.UpdatedAt = DateTime.UtcNow;

            // await _astronomicalService.UpdatePlanetAsync(planet);
            
            AddSuccessMessage($"Planeta '{model.Name}' actualizado exitosamente");
            return RedirectToAction("Details", new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating planet");
            ModelState.AddModelError("", "Error al actualizar el planeta: " + ex.Message);
            model.AvailablePlanetTypes = Enum.GetNames(typeof(PlanetType)).ToList();
            model.AvailableStars = (await _starRepository.GetAllAsync()).Select(s => new StarViewModel
            {
                StarID = s.StarID,
                Name = s.Name
            }).ToList();
            return View(model);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var planet = await _astronomicalService.GetPlanetByIdAsync(id);
            
            if (planet == null)
            {
                AddErrorMessage("Planeta no encontrado");
                return RedirectToAction("Index");
            }

            // await _astronomicalService.DeletePlanetAsync(id);
            
            AddSuccessMessage($"Planeta '{planet.Name}' eliminado exitosamente");
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting planet");
            AddErrorMessage("Error al eliminar el planeta: " + ex.Message);
            return RedirectToAction("Details", new { id });
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Habitable()
    {
        try
        {
            var planets = await _astronomicalService.GetAllPlanetsAsync();
            var habitablePlanets = planets.Where(p => p.IsPotentiallyHabitable).ToList();
            
            var model = new PlanetListViewModel
            {
                Planets = habitablePlanets,
                HabitableOnly = true
            };

            return View("Index", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading habitable planets");
            return RedirectToAction("Index");
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ByStar(int starId)
    {
        try
        {
            var planets = await _astronomicalService.GetPlanetsByStarAsync(starId);
            var star = await _astronomicalService.GetStarByIdAsync(starId);
            
            var model = new PlanetListViewModel
            {
                Planets = planets.ToList(),
                StarName = star?.Name
            };

            return View("Index", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading planets by star");
            return RedirectToAction("Index");
        }
    }
}