namespace WatchTower.Core.Results;

public class ServiceResult<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public string ErrorMessage { get; }
    public string ErrorCode { get; }

    public ServiceResult(T data)
    {
        IsSuccess = true;
        Data = data;
        ErrorMessage = string.Empty;
        ErrorCode = string.Empty;
    }

    public ServiceResult(string errorMessage, string errorCode = "ERROR")
    {
        IsSuccess = false;
        Data = default;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
    }

    public static ServiceResult<T> Success(T data) => new ServiceResult<T>(data);
    public static ServiceResult<T> Failure(string errorMessage, string errorCode = "ERROR") 
        => new ServiceResult<T>(errorMessage, errorCode);
}