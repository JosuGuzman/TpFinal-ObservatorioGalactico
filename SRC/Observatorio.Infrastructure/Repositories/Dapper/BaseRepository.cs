namespace Observatorio.Infrastructure.Repositories.Dapper;

public abstract class BaseRepository
{
    protected readonly DapperContext _context;

    protected BaseRepository(DapperContext context)
    {
        _context = context;
    }

    protected async Task<T> WithConnection<T>(Func<IDbConnection, Task<T>> operation)
    {
        using var connection = _context.CreateConnection();
        return await operation(connection);
    }

    protected async Task WithConnection(Func<IDbConnection, Task> operation)
    {
        using var connection = _context.CreateConnection();
        await operation(connection);
    }
}