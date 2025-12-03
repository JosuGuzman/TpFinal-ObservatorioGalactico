using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Observatorio.Core.Entities.Astronomic;
using Observatorio.Core.Enums;
using Observatorio.Core.Interfaces;
using Observatorio.Mvc.Models.Astronomical;

namespace Observatorio.Mvc.Controllers.Astronomical;

[Authorize]
public class StarController : AstronomicalBaseController
{
    private readonly IGalaxyRepository _galaxyRepository;

    public StarController(
        IAstronomicalDataService astronomicalService,
        IGalaxyRepository galaxyRepository,
        ILogger<StarController> logger) : base(astronomicalService, logger)
    {
        _galaxyRepository = galaxyRepository;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(
        int page = 1,
        int pageSize = 20,
        string search = "",
        string spectralType = "",
        int? galaxyId = null,
        double? maxDistance = null,
        string sortBy = "name")
    {
        try
        {
            IEnumerable<Star> stars = await _astronomicalService.GetAllStarsAsync();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(search))
                stars = stars.Where(s => 
                    s.Name.Contains(search, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(spectralType))
                stars = stars.Where(s => s.SpectralType.ToString() == spectralType);

            if (galaxyId.HasValue)
                stars = stars.Where(s => s.GalaxyID == galaxyId);

            if (maxDistance.HasValue)
                stars = stars.Where(s => s.DistanceLy <= maxDistance.Value);

            // Ordenar
            stars = sortBy.ToLower() switch
            {
                "distance" => stars.OrderBy(s => s.DistanceLy),
                "temperature" => stars.OrderByDescending(s => s.SurfaceTempK),
                "mass" => stars.OrderByDescending(s => s.MassSolar),
                _ => stars.OrderBy(s => s.Name)
            };

            var totalCount = stars.Count();
            var pagedStars = stars
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new StarListViewModel
            {
                Stars = pagedStars,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                SearchQuery = search,
                FilterSpectralType = spectralType,
                FilterGalaxyId = galaxyId,
                FilterMaxDistance = maxDistance,
                SortBy = sortBy,
                AvailableSpectralTypes = Enum.GetNames(typeof(SpectralType)).ToList(),
                AvailableGalaxies = (await _galaxyRepository.GetAllAsync()).ToList()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading stars");
            AddErrorMessage("Error al cargar las estrellas");
            return View(new StarListViewModel());
        }
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var star = await _astronomicalService.GetStarByIdAsync(id);
            
            if (star == null)
            {
                AddErrorMessage("Estrella no encontrada");
                return RedirectToAction("Index");
            }

            // Obtener planetas de esta estrella
            var planets = await _astronomicalService.GetPlanetsByStarAsync(id);
            
            var model = new StarDetailViewModel
            {
                Star = star,
                Planets = planets.ToList(),
                PlanetCount = planets.Count(),
                CanEdit = IsAdmin() || IsAstronomer()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading star details");
            AddErrorMessage("Error al cargar los detalles de la estrella");
            return RedirectToAction("Index");
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Astronomer")]
    public async Task<IActionResult> Create()
    {
        var model = new StarViewModel
        {
            AvailableSpectralTypes = Enum.GetNames(typeof(SpectralType)).ToList(),
            AvailableGalaxies = (await _galaxyRepository.GetAllAsync()).Select(g => new GalaxyViewModel
            {
                GalaxyID = g.GalaxyID,
                Name = g.Name
            }).ToList()
        };
        
        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Astronomer")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StarViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableSpectralTypes = Enum.GetNames(typeof(SpectralType)).ToList();
            model.AvailableGalaxies = (await _galaxyRepository.GetAllAsync()).Select(g => new GalaxyViewModel
            {
                GalaxyID = g.GalaxyID,
                Name = g.Name
            }).ToList();
            return View(model);
        }

        try
        {
            var (isValid, message) = await ValidateCoordinates(model.RA, model.Dec);
            if (!isValid)
            {
                ModelState.AddModelError("", message);
                model.AvailableSpectralTypes = Enum.GetNames(typeof(SpectralType)).ToList();
                model.AvailableGalaxies = (await _galaxyRepository.GetAllAsync()).Select(g => new GalaxyViewModel
                {
                    GalaxyID = g.GalaxyID,
                    Name = g.Name
                }).ToList();
                return View(model);
            }

            var star = new Star
            {
                GalaxyID = model.GalaxyID,
                Name = model.Name,
                SpectralType = model.SpectralType,
                SurfaceTempK = model.SurfaceTempK,
                MassSolar = model.MassSolar,
                RadiusSolar = model.RadiusSolar,
                LuminositySolar = model.LuminositySolar,
                DistanceLy = model.DistanceLy,
                RA = model.RA,
                Dec = model.Dec,
                RadialVelocity = model.RadialVelocity,
                ApparentMagnitude = model.ApparentMagnitude,
                EstimatedAgeMillionYears = model.EstimatedAgeMillionYears,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // await _astronomicalService.CreateStarAsync(star);
            
            AddSuccessMessage($"Estrella '{model.Name}' creada exitosamente");
            return RedirectToAction("Details", new { id = star.StarID });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating star");
            ModelState.AddModelError("", "Error al crear la estrella: " + ex.Message);
            model.AvailableSpectralTypes = Enum.GetNames(typeof(SpectralType)).ToList();
            model.AvailableGalaxies = (await _galaxyRepository.GetAllAsync()).Select(g => new GalaxyViewModel
            {
                GalaxyID = g.GalaxyID,
                Name = g.Name
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
            var star = await _astronomicalService.GetStarByIdAsync(id);
            
            if (star == null)
            {
                AddErrorMessage("Estrella no encontrada");
                return RedirectToAction("Index");
            }

            var model = new StarViewModel
            {
                StarID = star.StarID,
                GalaxyID = star.GalaxyID,
                Name = star.Name,
                SpectralType = star.SpectralType,
                SurfaceTempK = star.SurfaceTempK,
                MassSolar = star.MassSolar,
                RadiusSolar = star.RadiusSolar,
                LuminositySolar = star.LuminositySolar,
                DistanceLy = star.DistanceLy,
                RA = star.RA,
                Dec = star.Dec,
                RadialVelocity = star.RadialVelocity,
                ApparentMagnitude = star.ApparentMagnitude,
                EstimatedAgeMillionYears = star.EstimatedAgeMillionYears,
                AvailableSpectralTypes = Enum.GetNames(typeof(SpectralType)).ToList(),
                AvailableGalaxies = (await _galaxyRepository.GetAllAsync()).Select(g => new GalaxyViewModel
                {
                    GalaxyID = g.GalaxyID,
                    Name = g.Name
                }).ToList()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading star for edit");
            AddErrorMessage("Error al cargar la estrella para editar");
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Astronomer")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, StarViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableSpectralTypes = Enum.GetNames(typeof(SpectralType)).ToList();
            model.AvailableGalaxies = (await _galaxyRepository.GetAllAsync()).Select(g => new GalaxyViewModel
            {
                GalaxyID = g.GalaxyID,
                Name = g.Name
            }).ToList();
            return View(model);
        }

        try
        {
            var star = await _astronomicalService.GetStarByIdAsync(id);
            
            if (star == null)
            {
                AddErrorMessage("Estrella no encontrada");
                return RedirectToAction("Index");
            }

            star.GalaxyID = model.GalaxyID;
            star.Name = model.Name;
            star.SpectralType = model.SpectralType;
            star.SurfaceTempK = model.SurfaceTempK;
            star.MassSolar = model.MassSolar;
            star.RadiusSolar = model.RadiusSolar;
            star.LuminositySolar = model.LuminositySolar;
            star.DistanceLy = model.DistanceLy;
            star.RA = model.RA;
            star.Dec = model.Dec;
            star.RadialVelocity = model.RadialVelocity;
            star.ApparentMagnitude = model.ApparentMagnitude;
            star.EstimatedAgeMillionYears = model.EstimatedAgeMillionYears;
            star.UpdatedAt = DateTime.UtcNow;

            // await _astronomicalService.UpdateStarAsync(star);
            
            AddSuccessMessage($"Estrella '{model.Name}' actualizada exitosamente");
            return RedirectToAction("Details", new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating star");
            ModelState.AddModelError("", "Error al actualizar la estrella: " + ex.Message);
            model.AvailableSpectralTypes = Enum.GetNames(typeof(SpectralType)).ToList();
            model.AvailableGalaxies = (await _galaxyRepository.GetAllAsync()).Select(g => new GalaxyViewModel
            {
                GalaxyID = g.GalaxyID,
                Name = g.Name
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
            var star = await _astronomicalService.GetStarByIdAsync(id);
            
            if (star == null)
            {
                AddErrorMessage("Estrella no encontrada");
                return RedirectToAction("Index");
            }

            // await _astronomicalService.DeleteStarAsync(id);
            
            AddSuccessMessage($"Estrella '{star.Name}' eliminada exitosamente");
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting star");
            AddErrorMessage("Error al eliminar la estrella: " + ex.Message);
            return RedirectToAction("Details", new { id });
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ByGalaxy(int galaxyId)
    {
        try
        {
            var stars = await _astronomicalService.GetStarsByGalaxyAsync(galaxyId);
            var galaxy = await _astronomicalService.GetGalaxyByIdAsync(galaxyId);
            
            var model = new StarListViewModel
            {
                Stars = stars.ToList(),
                GalaxyName = galaxy?.Name
            };

            return View("Index", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading stars by galaxy");
            return RedirectToAction("Index");
        }
    }
}