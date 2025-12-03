namespace Observatorio.Infrastructure.Data;

public class DapperContext
{
    private readonly DatabaseConfig _databaseConfig;

    public DapperContext(IOptions<DatabaseConfig> databaseConfig)
    {
        _databaseConfig = databaseConfig.Value;
    }

    public IDbConnection CreateConnection()
    {
        var connection = new MySqlConnection(_databaseConfig.ConnectionString);
        connection.Open();
        return connection;
    }

    public async Task<IDbConnection> CreateConnectionAsync()
    {
        var connection = new MySqlConnection(_databaseConfig.ConnectionString);
        await connection.OpenAsync();
        return connection;
    }
}