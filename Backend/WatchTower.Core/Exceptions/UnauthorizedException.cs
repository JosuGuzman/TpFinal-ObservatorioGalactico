namespace WatchTower.Core.Exceptions;

public class UnauthorizedException : WatchTowerException
{
    public UnauthorizedException(string message = "Unauthorized access") 
        : base(message, "UNAUTHORIZED", 401)
    {
    }
}