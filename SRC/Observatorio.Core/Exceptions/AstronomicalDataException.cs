namespace Observatorio.Core.Exceptions;

public class AstronomicalDataException : Exception
{
    public AstronomicalDataException() { }
    public AstronomicalDataException(string message) : base(message) { }
    public AstronomicalDataException(string message, Exception innerException) : base(message, innerException) { }
}