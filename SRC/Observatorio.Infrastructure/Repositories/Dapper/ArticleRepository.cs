namespace Observatorio.Infrastructure.Repositories.Dapper;

public class ArticleRepository : BaseRepository, IArticleRepository
{
    public ArticleRepository(DapperContext context) : base(context)
    {
    }

    public async Task<Article> GetByIdAsync(int id)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT a.*, u.* FROM Articles a
                LEFT JOIN Users u ON a.AuthorUserID = u.UserID
                WHERE a.ArticleID = @id";

            var result = await conn.QueryAsync<Article, User, Article>(
                sql,
                (article, user) =>
                {
                    article.Author = user;
                    return article;
                },
                new { id },
                splitOn: "UserID"
            );

            return result.FirstOrDefault();
        });
    }

    public async Task<IEnumerable<Article>> GetAllAsync()
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT a.*, u.* FROM Articles a
                LEFT JOIN Users u ON a.AuthorUserID = u.UserID
                ORDER BY a.CreatedAt DESC";

            var result = await conn.QueryAsync<Article, User, Article>(
                sql,
                (article, user) =>
                {
                    article.Author = user;
                    return article;
                },
                splitOn: "UserID"
            );

            return result;
        });
    }

    public async Task<Article> AddAsync(Article entity)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                CALL sp_create_article(
                    @Title, @Slug, @Content, @AuthorUserID
                );
                SELECT LAST_INSERT_ID();";

            var parameters = new
            {
                entity.Title,
                entity.Slug,
                entity.Content,
                entity.AuthorUserID
            };

            var id = await conn.ExecuteScalarAsync<int>(sql, parameters);
            entity.ArticleID = id;
            return entity;
        });
    }

    public async Task UpdateAsync(Article entity)
    {
        await WithConnection(async conn =>
        {
            var sql = @"
                CALL sp_update_article(
                    @ArticleID, @Title, @Content, @State
                )";

            var parameters = new
            {
                entity.ArticleID,
                entity.Title,
                entity.Content,
                State = entity.State.ToString()
            };

            await conn.ExecuteAsync(sql, parameters);
        });
    }

    public async Task DeleteAsync(int id)
    {
        await WithConnection(async conn =>
        {
            var sql = "CALL sp_delete_article(@id)";
            await conn.ExecuteAsync(sql, new { id });
        });
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(1) FROM Articles WHERE ArticleID = @id";
            var count = await conn.ExecuteScalarAsync<int>(sql, new { id });
            return count > 0;
        });
    }

    public async Task<int> CountAsync()
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(*) FROM Articles";
            return await conn.ExecuteScalarAsync<int>(sql);
        });
    }

    public async Task<IEnumerable<Article>> GetByAuthorAsync(int authorId)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT a.*, u.* FROM Articles a
                LEFT JOIN Users u ON a.AuthorUserID = u.UserID
                WHERE a.AuthorUserID = @authorId
                ORDER BY a.CreatedAt DESC";

            var result = await conn.QueryAsync<Article, User, Article>(
                sql,
                (article, user) =>
                {
                    article.Author = user;
                    return article;
                },
                new { authorId },
                splitOn: "UserID"
            );

            return result;
        });
    }

    public async Task<IEnumerable<Article>> GetPublishedAsync()
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT a.*, u.* FROM Articles a
                LEFT JOIN Users u ON a.AuthorUserID = u.UserID
                WHERE a.State = 'Published' AND a.PublishedAt IS NOT NULL
                ORDER BY a.PublishedAt DESC";

            var result = await conn.QueryAsync<Article, User, Article>(
                sql,
                (article, user) =>
                {
                    article.Author = user;
                    return article;
                },
                splitOn: "UserID"
            );

            return result;
        });
    }

    public async Task<IEnumerable<Article>> GetByStateAsync(string state)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT a.*, u.* FROM Articles a
                LEFT JOIN Users u ON a.AuthorUserID = u.UserID
                WHERE a.State = @state
                ORDER BY a.CreatedAt DESC";

            var result = await conn.QueryAsync<Article, User, Article>(
                sql,
                (article, user) =>
                {
                    article.Author = user;
                    return article;
                },
                new { state },
                splitOn: "UserID"
            );

            return result;
        });
    }

    public async Task<IEnumerable<Article>> SearchByTitleAsync(string title)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT a.*, u.* FROM Articles a
                LEFT JOIN Users u ON a.AuthorUserID = u.UserID
                WHERE MATCH(a.Title, a.Content) AGAINST(@title IN NATURAL LANGUAGE MODE)
                ORDER BY a.CreatedAt DESC";

            var result = await conn.QueryAsync<Article, User, Article>(
                sql,
                (article, user) =>
                {
                    article.Author = user;
                    return article;
                },
                new { title },
                splitOn: "UserID"
            );

            return result;
        });
    }

    public async Task<IEnumerable<Article>> GetLatestPublishedAsync(int limit)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT a.*, u.* FROM Articles a
                LEFT JOIN Users u ON a.AuthorUserID = u.UserID
                WHERE a.State = 'Published' AND a.PublishedAt IS NOT NULL
                ORDER BY a.PublishedAt DESC
                LIMIT @limit";

            var result = await conn.QueryAsync<Article, User, Article>(
                sql,
                (article, user) =>
                {
                    article.Author = user;
                    return article;
                },
                new { limit },
                splitOn: "UserID"
            );

            return result;
        });
    }

    public async Task<int> CountByStateAsync(string state)
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(*) FROM Articles WHERE State = @state";
            return await conn.ExecuteScalarAsync<int>(sql, new { state });
        });
    }
}