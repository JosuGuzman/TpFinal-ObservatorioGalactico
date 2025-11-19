using System.Data;
using MySql.Data.MySqlClient;

namespace WatchTower.API.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}

public class MySqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;
    
    public MySqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public IDbConnection CreateConnection() => new MySqlConnection(_connectionString);
}