namespace Observatorio.Infrastructure.Repositories.Dapper;

public class PlanetRepository : BaseRepository, IPlanetRepository
{
    public PlanetRepository(DapperContext context) : base(context)
    {
    }

    public async Task<Planet> GetByIdAsync(int id)
    {
        return await WithConnection(async conn =>
        {
            var sql = "CALL sp_get_planet_by_id(@id)";
            return await conn.QueryFirstOrDefaultAsync<Planet>(sql, new { id });
        });
    }

    public async Task<IEnumerable<Planet>> GetAllAsync()
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT * FROM Planets ORDER BY Name";
            return await conn.QueryAsync<Planet>(sql);
        });
    }

    public async Task<Planet> AddAsync(Planet entity)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                CALL sp_insert_planet(
                    @StarID, @Name, @PlanetType, @MassEarth, @RadiusEarth, 
                    @OrbitalPeriodDays, @OrbitalDistanceAU, @Eccentricity, @HabitabilityScore
                );
                SELECT LAST_INSERT_ID();";

            var parameters = new
            {
                entity.StarID,
                entity.Name,
                PlanetType = entity.PlanetType.ToString(),
                entity.MassEarth,
                entity.RadiusEarth,
                entity.OrbitalPeriodDays,
                entity.OrbitalDistanceAU,
                entity.Eccentricity,
                entity.HabitabilityScore
            };

            var id = await conn.ExecuteScalarAsync<int>(sql, parameters);
            entity.PlanetID = id;
            return entity;
        });
    }

    public async Task UpdateAsync(Planet entity)
    {
        await WithConnection(async conn =>
        {
            var sql = @"
                CALL sp_update_planet(
                    @PlanetID, @Name, @PlanetType, @MassEarth, @RadiusEarth, 
                    @OrbitalPeriodDays, @OrbitalDistanceAU, @Eccentricity, @HabitabilityScore
                )";

            var parameters = new
            {
                entity.PlanetID,
                entity.Name,
                PlanetType = entity.PlanetType.ToString(),
                entity.MassEarth,
                entity.RadiusEarth,
                entity.OrbitalPeriodDays,
                entity.OrbitalDistanceAU,
                entity.Eccentricity,
                entity.HabitabilityScore
            };

            await conn.ExecuteAsync(sql, parameters);
        });
    }

    public async Task DeleteAsync(int id)
    {
        await WithConnection(async conn =>
        {
            var sql = "CALL sp_delete_planet(@id)";
            await conn.ExecuteAsync(sql, new { id });
        });
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(1) FROM Planets WHERE PlanetID = @id";
            var count = await conn.ExecuteScalarAsync<int>(sql, new { id });
            return count > 0;
        });
    }

    public async Task<int> CountAsync()
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(*) FROM Planets";
            return await conn.ExecuteScalarAsync<int>(sql);
        });
    }

    public async Task<IEnumerable<Planet>> GetByStarAsync(int starId)
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT * FROM Planets WHERE StarID = @starId ORDER BY Name";
            return await conn.QueryAsync<Planet>(sql, new { starId });
        });
    }

    public async Task<IEnumerable<Planet>> GetByPlanetTypeAsync(string planetType)
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT * FROM Planets WHERE PlanetType = @planetType ORDER BY Name";
            return await conn.QueryAsync<Planet>(sql, new { planetType });
        });
    }

    public async Task<IEnumerable<Planet>> GetHabitablesAsync(double minHabitability = 0.7)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT * FROM Planets 
                WHERE HabitabilityScore >= @minHabitability 
                ORDER BY HabitabilityScore DESC";

            return await conn.QueryAsync<Planet>(sql, new { minHabitability });
        });
    }

    public async Task<IEnumerable<Planet>> SearchByNameAsync(string name)
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT * FROM Planets WHERE Name LIKE @name ORDER BY Name";
            return await conn.QueryAsync<Planet>(sql, new { name = $"%{name}%" });
        });
    }

    public async Task<int> CountByStarAsync(int starId)
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(*) FROM Planets WHERE StarID = @starId";
            return await conn.ExecuteScalarAsync<int>(sql, new { starId });
        });
    }

    public async Task<int> CountByTypeAsync(string planetType)
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(*) FROM Planets WHERE PlanetType = @planetType";
            return await conn.ExecuteScalarAsync<int>(sql, new { planetType });
        });
    }
}