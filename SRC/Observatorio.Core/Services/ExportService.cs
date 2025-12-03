namespace Observatorio.Core.Services;

public class ExportService : IExportService
{
    private readonly IGalaxyRepository _galaxyRepository;
    private readonly IStarRepository _starRepository;
    private readonly IPlanetRepository _planetRepository;
    private readonly IDiscoveryRepository _discoveryRepository;
    private readonly IUserRepository _userRepository;
    
    public ExportService(
        IGalaxyRepository galaxyRepository,
        IStarRepository starRepository,
        IPlanetRepository planetRepository,
        IDiscoveryRepository discoveryRepository,
        IUserRepository userRepository)
    {
        _galaxyRepository = galaxyRepository;
        _starRepository = starRepository;
        _planetRepository = planetRepository;
        _discoveryRepository = discoveryRepository;
        _userRepository = userRepository;
    }

    public async Task<byte[]> ExportGalaxiesToCsvAsync()
    {
        var galaxies = await _galaxyRepository.GetAllAsync();
        var csvLines = new List<string>
        {
            "ID,Name,Type,Distance (ly),RA,Dec,ApparentMagnitude,Redshift,Description"
        };

        foreach (var galaxy in galaxies)
        {
            csvLines.Add($"\"{galaxy.GalaxyID}\"," +
                        $"\"{galaxy.Name}\"," +
                        $"\"{galaxy.Type}\"," +
                        $"{galaxy.DistanceLy}," +
                        $"{galaxy.RA}," +
                        $"{galaxy.Dec}," +
                        $"{galaxy.ApparentMagnitude?.ToString() ?? ""}," +
                        $"{galaxy.Redshift?.ToString() ?? ""}," +
                        $"\"{galaxy.Description?.Replace("\"", "\"\"") ?? ""}\"");
        }

        return Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, csvLines));
    }

    public async Task<byte[]> ExportStarsToCsvAsync()
    {
        var stars = await _starRepository.GetAllAsync();
        var csvLines = new List<string>
        {
            "ID,Name,GalaxyID,SpectralType,SurfaceTemp (K),Mass (Solar),Radius (Solar),Distance (ly),RA,Dec"
        };

        foreach (var star in stars)
        {
            csvLines.Add($"\"{star.StarID}\"," +
                        $"\"{star.Name}\"," +
                        $"{star.GalaxyID?.ToString() ?? ""}," +
                        $"\"{star.SpectralType}\"," +
                        $"{star.SurfaceTempK?.ToString() ?? ""}," +
                        $"{star.MassSolar?.ToString() ?? ""}," +
                        $"{star.RadiusSolar?.ToString() ?? ""}," +
                        $"{star.DistanceLy?.ToString() ?? ""}," +
                        $"{star.RA}," +
                        $"{star.Dec}");
        }

        return Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, csvLines));
    }

    public async Task<byte[]> ExportPlanetsToCsvAsync()
    {
        var planets = await _planetRepository.GetAllAsync();
        var csvLines = new List<string>
        {
            "ID,Name,StarID,PlanetType,Mass (Earth),Radius (Earth),OrbitalPeriod (days),OrbitalDistance (AU),Habitability"
        };

        foreach (var planet in planets)
        {
            csvLines.Add($"\"{planet.PlanetID}\"," +
                        $"\"{planet.Name}\"," +
                        $"{planet.StarID}," +
                        $"\"{planet.PlanetType}\"," +
                        $"{planet.MassEarth?.ToString() ?? ""}," +
                        $"{planet.RadiusEarth?.ToString() ?? ""}," +
                        $"{planet.OrbitalPeriodDays?.ToString() ?? ""}," +
                        $"{planet.OrbitalDistanceAU?.ToString() ?? ""}," +
                        $"{planet.HabitabilityScore?.ToString() ?? ""}");
        }

        return Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, csvLines));
    }

    public async Task<byte[]> ExportDiscoveriesToCsvAsync()
    {
        var discoveries = await _discoveryRepository.GetAllAsync();
        var csvLines = new List<string>
        {
            "ID,ReporterUserID,ObjectType,SuggestedName,RA,Dec,State,Description,CreatedAt"
        };

        foreach (var discovery in discoveries)
        {
            csvLines.Add($"\"{discovery.DiscoveryID}\"," +
                        $"{discovery.ReporterUserID}," +
                        $"\"{discovery.ObjectType}\"," +
                        $"\"{discovery.SuggestedName}\"," +
                        $"{discovery.RA}," +
                        $"{discovery.Dec}," +
                        $"\"{discovery.State}\"," +
                        $"\"{discovery.Description?.Replace("\"", "\"\"") ?? ""}\"," +
                        $"{discovery.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        }

        return Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, csvLines));
    }

    public async Task<byte[]> ExportUsersToCsvAsync()
    {
        var users = await _userRepository.GetAllAsync();
        var csvLines = new List<string>
        {
            "ID,Email,UserName,RoleID,IsActive,CreatedAt,LastLogin"
        };

        foreach (var user in users)
        {
            csvLines.Add($"\"{user.UserID}\"," +
                        $"\"{user.Email}\"," +
                        $"\"{user.UserName}\"," +
                        $"{user.RoleID}," +
                        $"{(user.IsActive ? "Yes" : "No")}," +
                        $"{user.CreatedAt:yyyy-MM-dd HH:mm:ss}," +
                        $"{(user.LastLogin.HasValue ? user.LastLogin.Value.ToString("yyyy-MM-dd HH:mm:ss") : "")}");
        }

        return Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, csvLines));
    }

    public Task<byte[]> ExportToPdfAsync(string dataType, int? id = null)
    {
        // Implementación real requeriría una biblioteca como iTextSharp o QuestPDF
        // Por ahora, devolvemos un PDF básico con texto
        var pdfContent = $"Report: {dataType}\nGenerated: {DateTime.UtcNow}\n";
        
        if (id.HasValue)
            pdfContent += $"ID: {id.Value}\n";
        
        var pdfBytes = Encoding.UTF8.GetBytes(pdfContent);
        return Task.FromResult(pdfBytes);
    }

    public async Task<string> ExportToJsonAsync(string dataType, int? id = null)
    {
        object data = dataType.ToLower() switch
        {
            "galaxies" => await _galaxyRepository.GetAllAsync(),
            "stars" => await _starRepository.GetAllAsync(),
            "planets" => await _planetRepository.GetAllAsync(),
            "discoveries" => await _discoveryRepository.GetAllAsync(),
            "users" => await _userRepository.GetAllAsync(),
            _ => throw new ArgumentException($"Unknown data type: {dataType}")
        };

        return JsonHelpers.Serialize(data);
    }

    public async Task<string> GenerateReportAsync(string reportType, DateTime? startDate = null, DateTime? endDate = null)
    {
        var report = new StringBuilder();
        report.AppendLine($"Report Type: {reportType}");
        report.AppendLine($"Period: {startDate?.ToString("yyyy-MM-dd") ?? "All time"} to {endDate?.ToString("yyyy-MM-dd") ?? "Present"}");
        report.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
        report.AppendLine("=".PadRight(50, '='));

        switch (reportType.ToLower())
        {
            case "summary":
                report.AppendLine($"Total Galaxies: {await _galaxyRepository.CountAsync()}");
                report.AppendLine($"Total Stars: {await _starRepository.CountAsync()}");
                report.AppendLine($"Total Planets: {await _planetRepository.CountAsync()}");
                report.AppendLine($"Total Discoveries: {await _discoveryRepository.CountAsync()}");
                report.AppendLine($"Pending Discoveries: {await _discoveryRepository.CountByStateAsync("Pendiente")}");
                break;

            case "discoveries":
                var discoveries = await _discoveryRepository.GetAllAsync();
                foreach (var discovery in discoveries)
                {
                    report.AppendLine($"- {discovery.SuggestedName} ({discovery.ObjectType}): {discovery.State}");
                }
                break;

            default:
                throw new ArgumentException($"Unknown report type: {reportType}");
        }

        return report.ToString();
    }
}