namespace Observatorio.Core.Exceptions;

public class ValidationException : Exception
{
    public List<string> Errors { get; }
    
    public ValidationException() : base("Validation failed") => Errors = new List<string>();
    public ValidationException(string message) : base(message) => Errors = new List<string> { message };
    public ValidationException(List<string> errors) : base("Validation failed") => Errors = errors ?? new List<string>();
    public ValidationException(string message, Exception innerException) : base(message, innerException) 
        => Errors = new List<string> { message };
}