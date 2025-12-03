namespace Observatorio.Core.Exceptions;

public class DatabaseException : Exception
{
    public string SqlQuery { get; }
    
    public DatabaseException() { }
    public DatabaseException(string message) : base(message) { }
    public DatabaseException(string message, string sqlQuery) : base(message) => SqlQuery = sqlQuery;
    public DatabaseException(string message, Exception innerException) : base(message, innerException) { }
}