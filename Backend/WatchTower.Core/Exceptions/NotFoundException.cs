namespace WatchTower.Core.Exceptions;

public class NotFoundException : WatchTowerException
{
    public NotFoundException(string resourceName, object resourceId) 
        : base($"No se encontró {resourceName} con ID {resourceId}", "NOT_FOUND", 404)
    {
    }
    
    public NotFoundException(string message) 
        : base(message, "NOT_FOUND", 404)
    {
    }
}