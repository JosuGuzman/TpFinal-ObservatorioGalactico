namespace Observatorio.Core.Exceptions;

public class ExternalApiException : Exception
{
    public string ApiName { get; }
    public int StatusCode { get; }
    
    public ExternalApiException(string apiName, string message, int statusCode) 
        : base($"{apiName} API error: {message}")
    {
        ApiName = apiName;
        StatusCode = statusCode;
    }
    
    public ExternalApiException(string apiName, string message, Exception innerException) 
        : base($"{apiName} API error: {message}", innerException) => ApiName = apiName;
}