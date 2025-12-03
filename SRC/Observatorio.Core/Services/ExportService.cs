namespace Observatorio.Core.Services;

public class ExportService : IExportService
{
    private readonly IAstronomicalDataService _astronomicalDataService;
    private readonly ILoggingService _loggingService;

    public ExportService(
        IAstronomicalDataService astronomicalDataService,
        ILoggingService loggingService)
    {
        _astronomicalDataService = astronomicalDataService;
        _loggingService = loggingService;
    }

    public async Task<byte[]> ExportGalaxiesToCsvAsync()
    {
        try
        {
            var galaxies = await _astronomicalDataService.GetAllGalaxiesAsync();
            
            var csv = new StringBuilder();
            csv.AppendLine("ID,Name,Type,Distance (ly),RA,Dec,Description");
            
            foreach (var galaxy in galaxies)
            {
                var line = $"\"{galaxy.GalaxyID}\",\"{galaxy.Name}\",\"{galaxy.Type}\",\"{galaxy.DistanceLy}\",\"{galaxy.RA}\",\"{galaxy.Dec}\",\"{galaxy.Description}\"";
                csv.AppendLine(line);
            }

            await _loggingService.LogInfoAsync("Export", "Galaxies exported to CSV", null);
            return Encoding.UTF8.GetBytes(csv.ToString());
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync("Export", "Error exporting galaxies to CSV", null, null, ex);
            throw;
        }
    }

    public async Task<byte[]> ExportStarsToCsvAsync()
    {
        try
        {
            var stars = await _astronomicalDataService.SearchStarsAsync("");
            
            var csv = new StringBuilder();
            csv.AppendLine("ID,Name,SpectralType,Temperature (K),Mass (Solar),Radius (Solar),Distance (ly),RA,Dec");
            
            foreach (var star in stars)
            {
                var line = $"\"{star.StarID}\",\"{star.Name}\",\"{star.SpectralType}\",\"{star.SurfaceTempK}\",\"{star.MassSolar}\",\"{star.RadiusSolar}\",\"{star.DistanceLy}\",\"{star.RA}\",\"{star.Dec}\"";
                csv.AppendLine(line);
            }

            await _loggingService.LogInfoAsync("Export", "Stars exported to CSV", null);
            return Encoding.UTF8.GetBytes(csv.ToString());
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync("Export", "Error exporting stars to CSV", null, null, ex);
            throw;
        }
    }

    public async Task<byte[]> ExportPlanetsToCsvAsync()
    {
        try
        {
            var planets = await _astronomicalDataService.SearchPlanetsAsync("");
            
            var csv = new StringBuilder();
            csv.AppendLine("ID,Name,Type,Mass (Earth),Radius (Earth),Orbital Distance (AU),Temperature (K),Habitability");
            
            foreach (var planet in planets)
            {
                var line = $"\"{planet.PlanetID}\",\"{planet.Name}\",\"{planet.PlanetType}\",\"{planet.MassEarth}\",\"{planet.RadiusEarth}\",\"{planet.OrbitalDistanceAU}\",\"{planet.SurfaceTempK}\",\"{planet.HabitabilityScore}\"";
                csv.AppendLine(line);
            }

            await _loggingService.LogInfoAsync("Export", "Planets exported to CSV", null);
            return Encoding.UTF8.GetBytes(csv.ToString());
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync("Export", "Error exporting planets to CSV", null, null, ex);
            throw;
        }
    }

    public async Task<byte[]> ExportDiscoveriesToCsvAsync()
    {
        // Este método requiere acceso a IDiscoveryRepository
        // Por ahora devolvemos un array vacío
        await _loggingService.LogInfoAsync("Export", "Discoveries export requested (not implemented)", null);
        return Array.Empty<byte>();
    }

    public async Task<byte[]> ExportUsersToCsvAsync()
    {
        // Este método requiere acceso a IUserRepository
        // Por ahora devolvemos un array vacío
        await _loggingService.LogInfoAsync("Export", "Users export requested (not implemented)", null);
        return Array.Empty<byte>();
    }

    public async Task<byte[]> ExportToPdfAsync(string dataType, int? id = null)
    {
        // Implementación de exportación a PDF (requiere librería como iTextSharp)
        await _loggingService.LogInfoAsync("Export", $"PDF export requested for {dataType} (ID: {id})", null);
        return Array.Empty<byte>();
    }

    public async Task<string> ExportToJsonAsync(string dataType, int? id = null)
    {
        try
        {
            object data = null;

            switch (dataType.ToLower())
            {
                case "galaxy":
                    if (id.HasValue)
                        data = await _astronomicalDataService.GetGalaxyByIdAsync(id.Value);
                    else
                        data = await _astronomicalDataService.GetAllGalaxiesAsync();
                    break;

                case "star":
                    if (id.HasValue)
                        data = await _astronomicalDataService.GetStarByIdAsync(id.Value);
                    else
                        data = await _astronomicalDataService.SearchStarsAsync("");
                    break;

                case "planet":
                    if (id.HasValue)
                        data = await _astronomicalDataService.GetPlanetByIdAsync(id.Value);
                    else
                        data = await _astronomicalDataService.SearchPlanetsAsync("");
                    break;

                default:
                    throw new ArgumentException($"Unsupported data type: {dataType}");
            }

            var json = JsonHelpers.Serialize(data);
            await _loggingService.LogInfoAsync("Export", $"{dataType} exported to JSON (ID: {id})", null);
            
            return json;
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync("Export", $"Error exporting {dataType} to JSON", null, null, ex);
            throw;
        }
    }

    public async Task<string> GenerateReportAsync(string reportType, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var report = new StringBuilder();
            report.AppendLine($"Report Type: {reportType}");
            report.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
            
            if (startDate.HasValue && endDate.HasValue)
            {
                report.AppendLine($"Period: {startDate.Value:yyyy-MM-dd} to {endDate.Value:yyyy-MM-dd}");
            }

            // Aquí agregarías la lógica específica del reporte
            switch (reportType.ToLower())
            {
                case "summary":
                    var galaxyCount = await _astronomicalDataService.GetGalaxiesCountAsync();
                    report.AppendLine($"Total Galaxies: {galaxyCount}");
                    break;

                case "habitable":
                    var habitablePlanets = await _astronomicalDataService.GetHabitablesPlanetsAsync();
                    report.AppendLine($"Habitable Planets: {habitablePlanets.Count()}");
                    break;

                default:
                    report.AppendLine("Report type not implemented");
                    break;
            }

            await _loggingService.LogInfoAsync("Report", $"{reportType} report generated", null);
            return report.ToString();
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync("Report", $"Error generating {reportType} report", null, null, ex);
            throw;
        }
    }
}