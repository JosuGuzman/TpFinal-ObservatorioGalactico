namespace WatchTower.Core.Interfaces.Services;

public interface IExportService
{
    Task<byte[]> ExportDataAsync(ExportRequest request);
    Task<Stream> ExportDataStreamAsync(ExportRequest request);
}