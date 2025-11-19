using Dapper;
using WatchTower.API.Data;
using WatchTower.API.Models.Entities;

namespace WatchTower.API.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> GetByIdAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE UserId = @UserId", new { UserId = userId });
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Username = @Username", new { Username = username });
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Email = @Email", new { Email = email });
    }

    public async Task<int> CreateAsync(User user)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, Role) 
                    VALUES (@Username, @Email, @PasswordHash, @FirstName, @LastName, @Role);
                    SELECT LAST_INSERT_ID();";
        
        return await connection.ExecuteScalarAsync<int>(sql, user);
    }

    public async Task UpdateAsync(User user)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"UPDATE Users SET 
                    Username = @Username, Email = @Email, FirstName = @FirstName, 
                    LastName = @LastName, Role = @Role, IsActive = @IsActive, 
                    LastLogin = @LastLogin 
                    WHERE UserId = @UserId";
        
        await connection.ExecuteAsync(sql, user);
    }

    public async Task<bool> UserExistsAsync(string username, string email)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT COUNT(1) FROM Users WHERE Username = @Username OR Email = @Email";
        var count = await connection.ExecuteScalarAsync<int>(sql, new { Username = username, Email = email });
        return count > 0;
    }
}
