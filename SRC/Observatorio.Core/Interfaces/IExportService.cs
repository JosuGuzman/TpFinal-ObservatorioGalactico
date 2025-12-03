namespace Observatorio.Core.Interfaces;

public interface IExportService
{
    Task<byte[]> ExportGalaxiesToCsvAsync();
    Task<byte[]> ExportStarsToCsvAsync();
    Task<byte[]> ExportPlanetsToCsvAsync();
    Task<byte[]> ExportDiscoveriesToCsvAsync();
    Task<byte[]> ExportUsersToCsvAsync();
    
    Task<byte[]> ExportToPdfAsync(string dataType, int? id = null);
    Task<string> ExportToJsonAsync(string dataType, int? id = null);
    
    Task<string> GenerateReportAsync(string reportType, DateTime? startDate = null, DateTime? endDate = null);
}