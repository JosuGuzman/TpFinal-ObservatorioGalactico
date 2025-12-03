namespace Observatorio.Infrastructure.Repositories.Dapper;

public class StarRepository : BaseRepository, IStarRepository
{
    public StarRepository(DapperContext context) : base(context)
    {
    }

    public async Task<Star> GetByIdAsync(int id)
    {
        return await WithConnection(async conn =>
        {
            var sql = "CALL sp_get_star_by_id(@id)";
            return await conn.QueryFirstOrDefaultAsync<Star>(sql, new { id });
        });
    }

    public async Task<IEnumerable<Star>> GetAllAsync()
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT * FROM Stars ORDER BY Name";
            return await conn.QueryAsync<Star>(sql);
        });
    }

    public async Task<Star> AddAsync(Star entity)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                CALL sp_insert_star(
                    @GalaxyID, @Name, @SpectralType, @SurfaceTempK, @MassSolar, 
                    @RadiusSolar, @LuminositySolar, @DistanceLy, @RA, @Dec
                );
                SELECT LAST_INSERT_ID();";

            var parameters = new
            {
                entity.GalaxyID,
                entity.Name,
                SpectralType = entity.SpectralType.ToString(),
                entity.SurfaceTempK,
                entity.MassSolar,
                entity.RadiusSolar,
                entity.LuminositySolar,
                entity.DistanceLy,
                entity.RA,
                entity.Dec
            };

            var id = await conn.ExecuteScalarAsync<int>(sql, parameters);
            entity.StarID = id;
            return entity;
        });
    }

    public async Task UpdateAsync(Star entity)
    {
        await WithConnection(async conn =>
        {
            var sql = @"
                CALL sp_update_star(
                    @StarID, @Name, @SpectralType, @SurfaceTempK, @MassSolar, 
                    @RadiusSolar, @LuminositySolar, @DistanceLy, @RA, @Dec
                )";

            var parameters = new
            {
                entity.StarID,
                entity.Name,
                SpectralType = entity.SpectralType.ToString(),
                entity.SurfaceTempK,
                entity.MassSolar,
                entity.RadiusSolar,
                entity.LuminositySolar,
                entity.DistanceLy,
                entity.RA,
                entity.Dec
            };

            await conn.ExecuteAsync(sql, parameters);
        });
    }

    public async Task DeleteAsync(int id)
    {
        await WithConnection(async conn =>
        {
            var sql = "CALL sp_delete_star(@id)";
            await conn.ExecuteAsync(sql, new { id });
        });
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(1) FROM Stars WHERE StarID = @id";
            var count = await conn.ExecuteScalarAsync<int>(sql, new { id });
            return count > 0;
        });
    }

    public async Task<int> CountAsync()
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(*) FROM Stars";
            return await conn.ExecuteScalarAsync<int>(sql);
        });
    }

    public async Task<IEnumerable<Star>> GetByGalaxyAsync(int galaxyId)
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT * FROM Stars WHERE GalaxyID = @galaxyId ORDER BY Name";
            return await conn.QueryAsync<Star>(sql, new { galaxyId });
        });
    }

    public async Task<IEnumerable<Star>> GetBySpectralTypeAsync(string spectralType)
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT * FROM Stars WHERE SpectralType = @spectralType ORDER BY Name";
            return await conn.QueryAsync<Star>(sql, new { spectralType });
        });
    }

    public async Task<IEnumerable<Star>> SearchByNameAsync(string name)
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT * FROM Stars WHERE Name LIKE @name ORDER BY Name";
            return await conn.QueryAsync<Star>(sql, new { name = $"%{name}%" });
        });
    }

    public async Task<IEnumerable<Star>> GetNearbyAsync(double ra, double dec, double radius)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT * FROM Stars 
                WHERE SQRT(POW(RA - @ra, 2) + POW(Dec - @dec, 2)) <= @radius
                ORDER BY SQRT(POW(RA - @ra, 2) + POW(Dec - @dec, 2))";

            return await conn.QueryAsync<Star>(sql, new { ra, dec, radius });
        });
    }

    public async Task<int> CountByGalaxyAsync(int galaxyId)
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(*) FROM Stars WHERE GalaxyID = @galaxyId";
            return await conn.ExecuteScalarAsync<int>(sql, new { galaxyId });
        });
    }
}