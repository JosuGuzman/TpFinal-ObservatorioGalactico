namespace WatchTower.Core.Exceptions;

public class DuplicateException : WatchTowerException
{
    public DuplicateException(string resourceName) 
        : base($"{resourceName} already exists", "DUPLICATE_RESOURCE", 409)
    {
    }
}