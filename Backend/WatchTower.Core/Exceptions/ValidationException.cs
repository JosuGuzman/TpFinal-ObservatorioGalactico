namespace WatchTower.Core.Exceptions;

public class ValidationException : WatchTowerException
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException(Dictionary<string, string[]> errors) 
        : base("Se produjeron uno o más errores de validación", "VALIDATION_ERROR", 400)
    {
        Errors = errors;
    }
}
