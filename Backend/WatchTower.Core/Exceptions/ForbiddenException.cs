namespace WatchTower.Core.Exceptions;

public class ForbiddenException : WatchTowerException
{
    public ForbiddenException(string message = "Access forbidden") 
        : base(message, "FORBIDDEN", 403)
    {
    }
}