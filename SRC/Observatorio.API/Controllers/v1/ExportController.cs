using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Observatorio.Core.Interfaces;

namespace Observatorio.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class ExportController : BaseApiController
{
    private readonly IExportService _exportService;
    private readonly ILoggingService _loggingService;

    public ExportController(IExportService exportService, ILoggingService loggingService)
    {
        _exportService = exportService;
        _loggingService = loggingService;
    }

    [HttpGet("galaxies/csv")]
    [Authorize(Roles = "Researcher,Astronomer,Admin")]
    public async Task<IActionResult> ExportGalaxiesToCsv()
    {
        try
        {
            var csvBytes = await _exportService.ExportGalaxiesToCsvAsync();
            
            await _loggingService.LogInfoAsync("ExportGalaxiesCSV", 
                $"Galaxies exported to CSV by user {GetCurrentUserId()}", 
                GetCurrentUserId());

            return File(csvBytes, "text/csv", $"galaxies_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv");
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error exporting galaxies to CSV", ex);
        }
    }

    [HttpGet("stars/csv")]
    [Authorize(Roles = "Researcher,Astronomer,Admin")]
    public async Task<IActionResult> ExportStarsToCsv()
    {
        try
        {
            var csvBytes = await _exportService.ExportStarsToCsvAsync();
            
            await _loggingService.LogInfoAsync("ExportStarsCSV", 
                $"Stars exported to CSV by user {GetCurrentUserId()}", 
                GetCurrentUserId());

            return File(csvBytes, "text/csv", $"stars_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv");
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error exporting stars to CSV", ex);
        }
    }

    [HttpGet("planets/csv")]
    [Authorize(Roles = "Researcher,Astronomer,Admin")]
    public async Task<IActionResult> ExportPlanetsToCsv()
    {
        try
        {
            var csvBytes = await _exportService.ExportPlanetsToCsvAsync();
            
            await _loggingService.LogInfoAsync("ExportPlanetsCSV", 
                $"Planets exported to CSV by user {GetCurrentUserId()}", 
                GetCurrentUserId());

            return File(csvBytes, "text/csv", $"planets_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv");
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error exporting planets to CSV", ex);
        }
    }

    [HttpGet("discoveries/csv")]
    [Authorize(Roles = "Astronomer,Admin")]
    public async Task<IActionResult> ExportDiscoveriesToCsv()
    {
        try
        {
            var csvBytes = await _exportService.ExportDiscoveriesToCsvAsync();
            
            await _loggingService.LogInfoAsync("ExportDiscoveriesCSV", 
                $"Discoveries exported to CSV by user {GetCurrentUserId()}", 
                GetCurrentUserId());

            return File(csvBytes, "text/csv", $"discoveries_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv");
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error exporting discoveries to CSV", ex);
        }
    }

    [HttpGet("users/csv")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ExportUsersToCsv()
    {
        try
        {
            var csvBytes = await _exportService.ExportUsersToCsvAsync();
            
            await _loggingService.LogInfoAsync("ExportUsersCSV", 
                $"Users exported to CSV by admin {GetCurrentUserId()}", 
                GetCurrentUserId());

            return File(csvBytes, "text/csv", $"users_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv");
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error exporting users to CSV", ex);
        }
    }

    [HttpGet("{type}/pdf")]
    [Authorize(Roles = "Astronomer,Admin")]
    public async Task<IActionResult> ExportToPdf(string type, [FromQuery] int? id = null)
    {
        try
        {
            var pdfBytes = await _exportService.ExportToPdfAsync(type, id);
            
            var filename = id.HasValue 
                ? $"{type}_{id}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf" 
                : $"{type}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";
            
            await _loggingService.LogInfoAsync("ExportPDF", 
                $"{type} exported to PDF by user {GetCurrentUserId()}", 
                GetCurrentUserId());

            return File(pdfBytes, "application/pdf", filename);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse($"Error exporting {type} to PDF", ex);
        }
    }

    [HttpGet("{type}/json")]
    [Authorize(Roles = "Researcher,Astronomer,Admin")]
    public async Task<IActionResult> ExportToJson(string type, [FromQuery] int? id = null)
    {
        try
        {
            var json = await _exportService.ExportToJsonAsync(type, id);
            
            await _loggingService.LogInfoAsync("ExportJSON", 
                $"{type} exported to JSON by user {GetCurrentUserId()}", 
                GetCurrentUserId());

            return Content(json, "application/json");
        }
        catch (Exception ex)
        {
            return InternalErrorResponse($"Error exporting {type} to JSON", ex);
        }
    }

    [HttpGet("report/{reportType}")]
    [Authorize(Roles = "Astronomer,Admin")]
    public async Task<IActionResult> GenerateReport(string reportType, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var report = await _exportService.GenerateReportAsync(reportType, startDate, endDate);
            
            await _loggingService.LogInfoAsync("GenerateReport", 
                $"{reportType} report generated by user {GetCurrentUserId()}", 
                GetCurrentUserId());

            return Content(report, "text/plain");
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error generating report", ex);
        }
    }

    [HttpGet("bulk-export")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> BulkExport([FromQuery] string formats = "csv,json")
    {
        try
        {
            var formatList = formats.Split(',');
            var exportResults = new List<object>();
            
            foreach (var format in formatList)
            {
                switch (format.ToLower())
                {
                    case "csv":
                        exportResults.Add(new { type = "galaxies", format = "csv", status = "available" });
                        exportResults.Add(new { type = "stars", format = "csv", status = "available" });
                        exportResults.Add(new { type = "planets", format = "csv", status = "available" });
                        break;
                        
                    case "json":
                        exportResults.Add(new { type = "galaxies", format = "json", status = "available" });
                        exportResults.Add(new { type = "stars", format = "json", status = "available" });
                        exportResults.Add(new { type = "planets", format = "json", status = "available" });
                        break;
                        
                    case "pdf":
                        exportResults.Add(new { type = "summary", format = "pdf", status = "available" });
                        break;
                }
            }
            
            await _loggingService.LogInfoAsync("BulkExport", 
                $"Bulk export options requested by admin {GetCurrentUserId()}", 
                GetCurrentUserId());

            return SuccessResponse(new
            {
                availableFormats = exportResults,
                instructions = "Use individual endpoints to download each format",
                note = "Large exports may take several minutes to process"
            });
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error processing bulk export request", ex);
        }
    }
}