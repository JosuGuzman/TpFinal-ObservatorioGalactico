using WatchTower.Core.Enums;

namespace WatchTower.Core.DTOs;

public class ExportRequest
{
    public string Dataset { get; set; } = string.Empty; // 'discoveries', 'bodies', 'articles'
    public ExportFormat Format { get; set; } = ExportFormat.Json;
    public string? Filters { get; set; }
}