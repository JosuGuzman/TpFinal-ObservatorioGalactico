namespace WatchTower.Core.Results;

public class PagedServiceResult<T>
{
    public bool IsSuccess { get; }
    public PagedResult<T>? Data { get; }
    public string ErrorMessage { get; }
    public string ErrorCode { get; }

    public PagedServiceResult(PagedResult<T> data)
    {
        IsSuccess = true;
        Data = data;
        ErrorMessage = string.Empty;
        ErrorCode = string.Empty;
    }

    public PagedServiceResult(string errorMessage, string errorCode = "ERROR")
    {
        IsSuccess = false;
        Data = null;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
    }

    public static PagedServiceResult<T> Success(PagedResult<T> data) => new PagedServiceResult<T>(data);
    public static PagedServiceResult<T> Failure(string errorMessage, string errorCode = "ERROR") 
        => new PagedServiceResult<T>(errorMessage, errorCode);
}