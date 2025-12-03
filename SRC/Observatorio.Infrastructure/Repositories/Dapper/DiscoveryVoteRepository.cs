namespace Observatorio.Infrastructure.Repositories.Dapper;

public class DiscoveryVoteRepository : BaseRepository, IDiscoveryVoteRepository
{
    public DiscoveryVoteRepository(DapperContext context) : base(context)
    {
    }

    public async Task<DiscoveryVote> GetByIdAsync(int id)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT v.*, d.*, u.* FROM DiscoveryVotes v
                LEFT JOIN Discoveries d ON v.DiscoveryID = d.DiscoveryID
                LEFT JOIN Users u ON v.VoterUserID = u.UserID
                WHERE v.VoteID = @id";

            var result = await conn.QueryAsync<DiscoveryVote, Discovery, User, DiscoveryVote>(
                sql,
                (vote, discovery, user) =>
                {
                    vote.Discovery = discovery;
                    vote.Voter = user;
                    return vote;
                },
                new { id },
                splitOn: "DiscoveryID,UserID"
            );

            return result.FirstOrDefault();
        });
    }

    public async Task<IEnumerable<DiscoveryVote>> GetAllAsync()
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT v.*, d.*, u.* FROM DiscoveryVotes v
                LEFT JOIN Discoveries d ON v.DiscoveryID = d.DiscoveryID
                LEFT JOIN Users u ON v.VoterUserID = u.UserID
                ORDER BY v.CreatedAt DESC";

            var result = await conn.QueryAsync<DiscoveryVote, Discovery, User, DiscoveryVote>(
                sql,
                (vote, discovery, user) =>
                {
                    vote.Discovery = discovery;
                    vote.Voter = user;
                    return vote;
                },
                splitOn: "DiscoveryID,UserID"
            );

            return result;
        });
    }

    public async Task<DiscoveryVote> AddAsync(DiscoveryVote entity)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                INSERT INTO DiscoveryVotes (DiscoveryID, VoterUserID, Vote, Comment, CreatedAt)
                VALUES (@DiscoveryID, @VoterUserID, @Vote, @Comment, @CreatedAt);
                SELECT LAST_INSERT_ID();";

            var id = await conn.ExecuteScalarAsync<int>(sql, entity);
            entity.VoteID = id;
            return entity;
        });
    }

    public async Task UpdateAsync(DiscoveryVote entity)
    {
        await WithConnection(async conn =>
        {
            var sql = @"
                UPDATE DiscoveryVotes 
                SET Vote = @Vote,
                    Comment = @Comment
                WHERE VoteID = @VoteID";

            await conn.ExecuteAsync(sql, entity);
        });
    }

    public async Task DeleteAsync(int id)
    {
        await WithConnection(async conn =>
        {
            var sql = "DELETE FROM DiscoveryVotes WHERE VoteID = @id";
            await conn.ExecuteAsync(sql, new { id });
        });
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(1) FROM DiscoveryVotes WHERE VoteID = @id";
            var count = await conn.ExecuteScalarAsync<int>(sql, new { id });
            return count > 0;
        });
    }

    public async Task<int> CountAsync()
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(*) FROM DiscoveryVotes";
            return await conn.ExecuteScalarAsync<int>(sql);
        });
    }

    public async Task<DiscoveryVote> GetByUserAndDiscoveryAsync(int userId, int discoveryId)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT v.*, d.*, u.* FROM DiscoveryVotes v
                LEFT JOIN Discoveries d ON v.DiscoveryID = d.DiscoveryID
                LEFT JOIN Users u ON v.VoterUserID = u.UserID
                WHERE v.VoterUserID = @userId AND v.DiscoveryID = @discoveryId";

            var result = await conn.QueryAsync<DiscoveryVote, Discovery, User, DiscoveryVote>(
                sql,
                (vote, discovery, user) =>
                {
                    vote.Discovery = discovery;
                    vote.Voter = user;
                    return vote;
                },
                new { userId, discoveryId },
                splitOn: "DiscoveryID,UserID"
            );

            return result.FirstOrDefault();
        });
    }

    public async Task<bool> HasVotedAsync(int userId, int discoveryId)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT COUNT(1) FROM DiscoveryVotes 
                WHERE VoterUserID = @userId AND DiscoveryID = @discoveryId";

            var count = await conn.ExecuteScalarAsync<int>(sql, new { userId, discoveryId });
            return count > 0;
        });
    }

    public async Task<int> GetUpvotesCountAsync(int discoveryId)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT COUNT(*) FROM DiscoveryVotes 
                WHERE DiscoveryID = @discoveryId AND Vote = 1";

            return await conn.ExecuteScalarAsync<int>(sql, new { discoveryId });
        });
    }

    public async Task<int> GetDownvotesCountAsync(int discoveryId)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT COUNT(*) FROM DiscoveryVotes 
                WHERE DiscoveryID = @discoveryId AND Vote = 0";

            return await conn.ExecuteScalarAsync<int>(sql, new { discoveryId });
        });
    }

    public async Task DeleteByDiscoveryAsync(int discoveryId)
    {
        await WithConnection(async conn =>
        {
            var sql = "DELETE FROM DiscoveryVotes WHERE DiscoveryID = @discoveryId";
            await conn.ExecuteAsync(sql, new { discoveryId });
        });
    }
}