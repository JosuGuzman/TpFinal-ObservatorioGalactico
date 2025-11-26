namespace WatchTower.Core.Exceptions;

public abstract class WatchTowerException : Exception
{
    public string ErrorCode { get; }
    public int StatusCode { get; }

    protected WatchTowerException(string message, string errorCode, int statusCode) 
        : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }
}