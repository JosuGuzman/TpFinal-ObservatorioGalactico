namespace WatchTower.Core.Interfaces.Services;

public interface INASASyncService
{
    Task SyncCelestialBodiesAsync();
    Task SyncAstronomicalEventsAsync();
    Task<DateTime> GetLastSyncDateAsync();
}