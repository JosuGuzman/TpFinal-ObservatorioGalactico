using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Observatorio.Core.Entities.Astronomic;
using Observatorio.Core.Enums;
using Observatorio.Core.Interfaces;
using Observatorio.Mvc.Models.Astronomical;

namespace Observatorio.Mvc.Controllers.Astronomical;

[Authorize]
public class GalaxyController : AstronomicalBaseController
{
    public GalaxyController(
        IAstronomicalDataService astronomicalService,
        ILogger<GalaxyController> logger) : base(astronomicalService, logger)
    {
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(
        int page = 1,
        int pageSize = 20,
        string search = "",
        string type = "",
        double? minDistance = null,
        double? maxDistance = null,
        string sortBy = "name",
        bool sortDesc = false)
    {
        try
        {
            IEnumerable<Galaxy> galaxies = await _astronomicalService.GetAllGalaxiesAsync();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(search))
                galaxies = galaxies.Where(g => 
                    g.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    g.Description.Contains(search, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(type) && Enum.TryParse<GalaxyType>(type, out var galaxyType))
                galaxies = galaxies.Where(g => g.Type == galaxyType);

            if (minDistance.HasValue)
                galaxies = galaxies.Where(g => g.DistanceLy >= minDistance.Value);

            if (maxDistance.HasValue)
                galaxies = galaxies.Where(g => g.DistanceLy <= maxDistance.Value);

            // Ordenar
            galaxies = sortBy.ToLower() switch
            {
                "distance" => sortDesc ? galaxies.OrderByDescending(g => g.DistanceLy) : galaxies.OrderBy(g => g.DistanceLy),
                "name" => sortDesc ? galaxies.OrderByDescending(g => g.Name) : galaxies.OrderBy(g => g.Name),
                "type" => sortDesc ? galaxies.OrderByDescending(g => g.Type) : galaxies.OrderBy(g => g.Type),
                _ => galaxies.OrderBy(g => g.Name)
            };

            var totalCount = galaxies.Count();
            var pagedGalaxies = galaxies
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new GalaxyListViewModel
            {
                Galaxies = pagedGalaxies,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                SearchQuery = search,
                FilterType = type,
                FilterMinDistance = minDistance,
                FilterMaxDistance = maxDistance,
                SortBy = sortBy,
                SortDescending = sortDesc,
                AvailableTypes = Enum.GetNames(typeof(GalaxyType)).ToList()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading galaxies");
            AddErrorMessage("Error al cargar las galaxias");
            return View(new GalaxyListViewModel());
        }
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var galaxy = await _astronomicalService.GetGalaxyByIdAsync(id);
            
            if (galaxy == null)
            {
                AddErrorMessage("Galaxia no encontrada");
                return RedirectToAction("Index");
            }

            // Obtener estrellas de esta galaxia
            var stars = await _astronomicalService.GetStarsByGalaxyAsync(id);
            
            var model = new GalaxyDetailViewModel
            {
                Galaxy = galaxy,
                Stars = stars.ToList(),
                StarCount = stars.Count(),
                CanEdit = IsAdmin() || IsAstronomer()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading galaxy details");
            AddErrorMessage("Error al cargar los detalles de la galaxia");
            return RedirectToAction("Index");
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Astronomer")]
    public IActionResult Create()
    {
        var model = new GalaxyViewModel
        {
            AvailableTypes = Enum.GetNames(typeof(GalaxyType)).ToList()
        };
        
        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Astronomer")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GalaxyViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableTypes = Enum.GetNames(typeof(GalaxyType)).ToList();
            return View(model);
        }

        try
        {
            // Validar coordenadas
            var (isValid, message) = await ValidateCoordinates(model.RA, model.Dec);
            if (!isValid)
            {
                ModelState.AddModelError("", message);
                model.AvailableTypes = Enum.GetNames(typeof(GalaxyType)).ToList();
                return View(model);
            }

            // Validar distancia
            var (isValidDist, messageDist) = await ValidateDistance(model.DistanceLy);
            if (!isValidDist)
            {
                ModelState.AddModelError("DistanceLy", messageDist);
                model.AvailableTypes = Enum.GetNames(typeof(GalaxyType)).ToList();
                return View(model);
            }

            var galaxy = new Galaxy
            {
                Name = model.Name,
                Type = model.Type,
                DistanceLy = model.DistanceLy,
                ApparentMagnitude = model.ApparentMagnitude,
                RA = model.RA,
                Dec = model.Dec,
                Redshift = model.Redshift,
                Description = model.Description,
                Discoverer = model.Discoverer,
                YearDiscovery = model.YearDiscovery,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Aquí necesitarías un repositorio para crear galaxias
            // await _astronomicalService.CreateGalaxyAsync(galaxy);
            
            AddSuccessMessage($"Galaxia '{model.Name}' creada exitosamente");
            return RedirectToAction("Details", new { id = galaxy.GalaxyID });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating galaxy");
            ModelState.AddModelError("", "Error al crear la galaxia: " + ex.Message);
            model.AvailableTypes = Enum.GetNames(typeof(GalaxyType)).ToList();
            return View(model);
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Astronomer")]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var galaxy = await _astronomicalService.GetGalaxyByIdAsync(id);
            
            if (galaxy == null)
            {
                AddErrorMessage("Galaxia no encontrada");
                return RedirectToAction("Index");
            }

            var model = new GalaxyViewModel
            {
                GalaxyID = galaxy.GalaxyID,
                Name = galaxy.Name,
                Type = galaxy.Type,
                DistanceLy = galaxy.DistanceLy,
                ApparentMagnitude = galaxy.ApparentMagnitude,
                RA = galaxy.RA,
                Dec = galaxy.Dec,
                Redshift = galaxy.Redshift,
                Description = galaxy.Description,
                Discoverer = galaxy.Discoverer,
                YearDiscovery = galaxy.YearDiscovery,
                AvailableTypes = Enum.GetNames(typeof(GalaxyType)).ToList()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading galaxy for edit");
            AddErrorMessage("Error al cargar la galaxia para editar");
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Astronomer")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, GalaxyViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableTypes = Enum.GetNames(typeof(GalaxyType)).ToList();
            return View(model);
        }

        try
        {
            var galaxy = await _astronomicalService.GetGalaxyByIdAsync(id);
            
            if (galaxy == null)
            {
                AddErrorMessage("Galaxia no encontrada");
                return RedirectToAction("Index");
            }

            // Validar coordenadas
            var (isValid, message) = await ValidateCoordinates(model.RA, model.Dec);
            if (!isValid)
            {
                ModelState.AddModelError("", message);
                model.AvailableTypes = Enum.GetNames(typeof(GalaxyType)).ToList();
                return View(model);
            }

            galaxy.Name = model.Name;
            galaxy.Type = model.Type;
            galaxy.DistanceLy = model.DistanceLy;
            galaxy.ApparentMagnitude = model.ApparentMagnitude;
            galaxy.RA = model.RA;
            galaxy.Dec = model.Dec;
            galaxy.Redshift = model.Redshift;
            galaxy.Description = model.Description;
            galaxy.Discoverer = model.Discoverer;
            galaxy.YearDiscovery = model.YearDiscovery;
            galaxy.UpdatedAt = DateTime.UtcNow;

            // await _astronomicalService.UpdateGalaxyAsync(galaxy);
            
            AddSuccessMessage($"Galaxia '{model.Name}' actualizada exitosamente");
            return RedirectToAction("Details", new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating galaxy");
            ModelState.AddModelError("", "Error al actualizar la galaxia: " + ex.Message);
            model.AvailableTypes = Enum.GetNames(typeof(GalaxyType)).ToList();
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
            var galaxy = await _astronomicalService.GetGalaxyByIdAsync(id);
            
            if (galaxy == null)
            {
                AddErrorMessage("Galaxia no encontrada");
                return RedirectToAction("Index");
            }

            // await _astronomicalService.DeleteGalaxyAsync(id);
            
            AddSuccessMessage($"Galaxia '{galaxy.Name}' eliminada exitosamente");
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting galaxy");
            AddErrorMessage("Error al eliminar la galaxia: " + ex.Message);
            return RedirectToAction("Details", new { id });
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Types(string type)
    {
        try
        {
            var galaxies = await _astronomicalService.GetAllGalaxiesAsync();
            
            if (!string.IsNullOrEmpty(type) && Enum.TryParse<GalaxyType>(type, out var galaxyType))
                galaxies = galaxies.Where(g => g.Type == galaxyType);

            var model = new GalaxyListViewModel
            {
                Galaxies = galaxies.ToList(),
                FilterType = type
            };

            return View("Index", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading galaxies by type");
            return RedirectToAction("Index");
        }
    }
}