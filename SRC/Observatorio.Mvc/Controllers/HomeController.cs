using Microsoft.AspNetCore.Mvc;
using Observatorio.Core.Interfaces;
using Observatorio.Mvc.Models;
using Observatorio.Mvc.Models.Home;
using System.Diagnostics;

namespace Observatorio.Mvc.Controllers;

public class HomeController : BaseController
{
    private readonly ILogger<HomeController> _logger;
    private readonly IAstronomicalDataService _astronomicalService;
    private readonly IDiscoveryService _discoveryService;
    private readonly IContentService _contentService;
    private readonly IUserService _userService;

    public HomeController(
        ILogger<HomeController> logger,
        IAstronomicalDataService astronomicalService,
        IDiscoveryService discoveryService,
        IContentService contentService,
        IUserService userService)
    {
        _logger = logger;
        _astronomicalService = astronomicalService;
        _discoveryService = discoveryService;
        _contentService = contentService;
        _userService = userService;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var model = new HomeViewModel
            {
                FeaturedGalaxies = (await _astronomicalService.GetAllGalaxiesAsync()).Take(3).ToList(),
                RecentDiscoveries = (await _discoveryService.GetAllDiscoveriesAsync()).Take(5).ToList(),
                UpcomingEvents = (await _contentService.GetUpcomingEventsAsync(3)).ToList(),
                LatestArticles = (await _contentService.GetLatestArticlesAsync(3)).ToList(),
                GalaxyCount = await _astronomicalService.GetGalaxiesCountAsync(),
                StarCount = await _astronomicalService.GetStarsCountAsync(),
                PlanetCount = await _astronomicalService.GetPlanetsCountAsync(),
                DiscoveryCount = await _discoveryService.GetDiscoveriesCountAsync()
            };
            
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading home page");
            return View(new HomeViewModel());
        }
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Contact(ContactViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // Aquí iría la lógica para enviar el correo
        AddSuccessMessage("Gracias por contactarnos. Te responderemos pronto.");
        return RedirectToAction("Contact");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Terms()
    {
        return View();
    }

    public IActionResult FAQ()
    {
        return View();
    }

    public IActionResult Guide()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet("sitemap.xml")]
    public IActionResult Sitemap()
    {
        // Para un sitemap real, necesitarías generar XML
        return Content("Sitemap XML generado dinámicamente", "text/xml");
    }

    [HttpGet("robots.txt")]
    public IActionResult Robots()
    {
        var content = @"User-agent: *
Allow: /
Disallow: /admin/
Disallow: /account/
Sitemap: https://observatorio.watchtower/sitemap.xml";
        
        return Content(content, "text/plain");
    }
}