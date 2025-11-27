using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace WatchTower.Infrastructure.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
    string GetConnectionString();
}

public class MySqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public MySqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection()
    {
        return new MySqlConnection(_connectionString);
    }

    public string GetConnectionString()
    {
        return _connectionString;
    }
}