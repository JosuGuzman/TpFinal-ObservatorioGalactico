namespace WatchTower.Infrastructure.Services;

public class ExportService : IExportService
{
    private readonly ICelestialBodyRepository _celestialBodyRepository;
    private readonly IDiscoveryRepository _discoveryRepository;
    private readonly IArticleRepository _articleRepository;

    public ExportService(
        ICelestialBodyRepository celestialBodyRepository,
        IDiscoveryRepository discoveryRepository,
        IArticleRepository articleRepository)
    {
        _celestialBodyRepository = celestialBodyRepository;
        _discoveryRepository = discoveryRepository;
        _articleRepository = articleRepository;
    }

    public async Task<byte[]> ExportDataAsync(ExportRequest request)
    {
        using var stream = await ExportDataStreamAsync(request);
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }

    public async Task<Stream> ExportDataStreamAsync(ExportRequest request)
    {
        return request.Dataset.ToLower() switch
        {
            "discoveries" => await ExportDiscoveriesAsync(request),
            "bodies" => await ExportCelestialBodiesAsync(request),
            "articles" => await ExportArticlesAsync(request),
            _ => throw new NotFoundException($"Dataset '{request.Dataset}' not found")
        };
    }

    private async Task<Stream> ExportDiscoveriesAsync(ExportRequest request)
    {
        var discoveries = await _discoveryRepository.SearchAsync(new DiscoverySearchRequest());
        return FormatData(discoveries, request.Format);
    }

    private async Task<Stream> ExportCelestialBodiesAsync(ExportRequest request)
    {
        var bodies = await _celestialBodyRepository.SearchAsync(new CelestialBodySearchRequest());
        return FormatData(bodies, request.Format);
    }

    private async Task<Stream> ExportArticlesAsync(ExportRequest request)
    {
        var articles = await _articleRepository.SearchAsync(new ArticleSearchRequest { PublishedOnly = false });
        return FormatData(articles, request.Format);
    }

    private static Stream FormatData<T>(IEnumerable<T> data, ExportFormat format)
    {
        return format switch
        {
            ExportFormat.Json => FormatAsJson(data),
            ExportFormat.Csv => FormatAsCsv(data),
            ExportFormat.Xml => FormatAsXml(data),
            _ => throw new BusinessRuleException($"Unsupported export format: {format}")
        };
    }

    private static Stream FormatAsJson<T>(IEnumerable<T> data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        return new MemoryStream(Encoding.UTF8.GetBytes(json));
    }

    private static Stream FormatAsCsv<T>(IEnumerable<T> data)
    {
        var sb = new StringBuilder();
        var properties = typeof(T).GetProperties();

        // Headers
        sb.AppendLine(string.Join(",", properties.Select(p => p.Name)));

        // Rows
        foreach (var item in data)
        {
            var values = properties.Select(p =>
            {
                var value = p.GetValue(item);
                return value?.ToString()?.Replace(",", ";") ?? "";
            });
            sb.AppendLine(string.Join(",", values));
        }

        return new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
    }

    private static Stream FormatAsXml<T>(IEnumerable<T> data)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine($"<{typeof(T).Name}s>");

        foreach (var item in data)
        {
            sb.AppendLine($"<{typeof(T).Name}>");
            var properties = typeof(T).GetProperties();
            foreach (var prop in properties)
            {
                var value = prop.GetValue(item);
                sb.AppendLine($"  <{prop.Name}>{value}</{prop.Name}>");
            }
            sb.AppendLine($"</{typeof(T).Name}>");
        }

        sb.AppendLine($"</{typeof(T).Name}s>");

        return new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
    }
}