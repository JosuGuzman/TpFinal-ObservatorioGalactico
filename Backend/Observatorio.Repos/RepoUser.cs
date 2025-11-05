using Observatorio.Core.Interfaces;

namespace Observatorio.Repo.Repositories;

public class RepoUser : RepoBase, IRepoUser
{
    public UserRepository(string connectionString) : base(connectionString) { }

    public async Task<User> GetByIdAsync(int id)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Users WHERE UserId = @Id";
        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
    }

    public async Task<User> GetByUsernameAsync(string username)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Users WHERE Username = @Username";
        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Username = username });
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Users WHERE Email = @Email";
        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Users WHERE IsActive = true";
        return await connection.QueryAsync<User>(sql);
    }

    public async Task<int> CreateAsync(User user)
    {
        using var connection = CreateConnection();
        var sql = @"INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, Role) 
                        VALUES (@Username, @Email, @PasswordHash, @FirstName, @LastName, @Role);
                        SELECT LAST_INSERT_ID();";
        return await connection.ExecuteScalarAsync<int>(sql, user);
    }

    public async Task UpdateAsync(User user)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE Users SET Username = @Username, Email = @Email, FirstName = @FirstName, 
                        LastName = @LastName, Role = @Role, IsActive = @IsActive WHERE UserId = @UserId";
        await connection.ExecuteAsync(sql, user);
    }

    public async Task DeleteAsync(int id)
    {
        using var connection = CreateConnection();
        var sql = "UPDATE Users SET IsActive = false WHERE UserId = @Id";
        await connection.ExecuteAsync(sql, new { Id = id });
    }

    public async Task UpdateLastLoginAsync(int userId)
    {
        using var connection = CreateConnection();
        var sql = "UPDATE Users SET LastLogin = NOW() WHERE UserId = @UserId";
        await connection.ExecuteAsync(sql, new { UserId = userId });
    }

}