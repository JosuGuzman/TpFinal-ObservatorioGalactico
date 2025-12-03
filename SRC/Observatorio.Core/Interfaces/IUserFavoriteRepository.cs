namespace Observatorio.Core.Interfaces;

public interface IUserFavoriteRepository : IRepository<UserFavorite>
{
    Task<IEnumerable<UserFavorite>> GetByUserAsync(int userId);
    Task<UserFavorite> GetByUserAndObjectAsync(int userId, string objectType, int objectId);
    Task<bool> IsFavoritedAsync(int userId, string objectType, int objectId);
    Task<int> CountByObjectAsync(string objectType, int objectId);
}