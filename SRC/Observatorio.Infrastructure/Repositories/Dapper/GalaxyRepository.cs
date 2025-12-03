namespace Observatorio.Infrastructure.Repositories.Dapper;

public class GalaxyRepository : BaseRepository, IGalaxyRepository
{
    public GalaxyRepository(DapperContext context) : base(context)
    {
    }

    public async Task<Galaxy> GetByIdAsync(int id)
    {
        return await WithConnection(async conn =>
        {
            var sql = "CALL sp_get_galaxy_by_id(@id)";
            return await conn.QueryFirstOrDefaultAsync<Galaxy>(sql, new { id });
        });
    }

    public async Task<IEnumerable<Galaxy>> GetAllAsync()
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT * FROM Galaxies ORDER BY Name";
            return await conn.QueryAsync<Galaxy>(sql);
        });
    }

    public async Task<Galaxy> AddAsync(Galaxy entity)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                CALL sp_insert_galaxy(
                    @Name, @Type, @DistanceLy, @ApparentMagnitude, 
                    @RA, @Dec, @Description
                );
                SELECT LAST_INSERT_ID();";

            var parameters = new
            {
                entity.Name,
                Type = entity.Type.ToString(),
                entity.DistanceLy,
                entity.ApparentMagnitude,
                entity.RA,
                entity.Dec,
                entity.Description
            };

            var id = await conn.ExecuteScalarAsync<int>(sql, parameters);
            entity.GalaxyID = id;
            return entity;
        });
    }

    public async Task UpdateAsync(Galaxy entity)
    {
        await WithConnection(async conn =>
        {
            var sql = @"
                CALL sp_update_galaxy(
                    @GalaxyID, @Name, @Type, @DistanceLy, @ApparentMagnitude, 
                    @RA, @Dec, @Description
                )";

            var parameters = new
            {
                entity.GalaxyID,
                entity.Name,
                Type = entity.Type.ToString(),
                entity.DistanceLy,
                entity.ApparentMagnitude,
                entity.RA,
                entity.Dec,
                entity.Description
            };

            await conn.ExecuteAsync(sql, parameters);
        });
    }

    public async Task DeleteAsync(int id)
    {
        await WithConnection(async conn =>
        {
            var sql = "CALL sp_delete_galaxy(@id)";
            await conn.ExecuteAsync(sql, new { id });
        });
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(1) FROM Galaxies WHERE GalaxyID = @id";
            var count = await conn.ExecuteScalarAsync<int>(sql, new { id });
            return count > 0;
        });
    }

    public async Task<int> CountAsync()
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(*) FROM Galaxies";
            return await conn.ExecuteScalarAsync<int>(sql);
        });
    }

    public async Task<IEnumerable<Galaxy>> GetByTypeAsync(string type)
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT * FROM Galaxies WHERE Type = @type ORDER BY Name";
            return await conn.QueryAsync<Galaxy>(sql, new { type });
        });
    }

    public async Task<IEnumerable<Galaxy>> SearchByNameAsync(string name)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT * FROM Galaxies 
                WHERE MATCH(Name, Description) AGAINST(@name IN NATURAL LANGUAGE MODE)
                ORDER BY Name";

            return await conn.QueryAsync<Galaxy>(sql, new { name });
        });
    }

    public async Task<IEnumerable<Galaxy>> GetByDistanceRangeAsync(double minDistance, double maxDistance)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT * FROM Galaxies 
                WHERE DistanceLy BETWEEN @minDistance AND @maxDistance 
                ORDER BY DistanceLy";

            return await conn.QueryAsync<Galaxy>(sql, new { minDistance, maxDistance });
        });
    }

    public async Task<IEnumerable<Galaxy>> GetNearbyAsync(double ra, double dec, double radius)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT * FROM Galaxies 
                WHERE SQRT(POW(RA - @ra, 2) + POW(Dec - @dec, 2)) <= @radius
                ORDER BY SQRT(POW(RA - @ra, 2) + POW(Dec - @dec, 2))";

            return await conn.QueryAsync<Galaxy>(sql, new { ra, dec, radius });
        });
    }

    public async Task<int> CountByTypeAsync(string type)
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(*) FROM Galaxies WHERE Type = @type";
            return await conn.ExecuteScalarAsync<int>(sql, new { type });
        });
    }
}