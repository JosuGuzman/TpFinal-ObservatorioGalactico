namespace WatchTower.Core.Interfaces.Repositories;

public interface IFavoriteRepository
{
    Task<Favorite?> GetByIdAsync(int favoriteId);
    Task<IEnumerable<Favorite>> GetUserFavoritesAsync(int userId);
    Task<bool> AddFavoriteAsync(Favorite favorite);
    Task<bool> RemoveFavoriteAsync(int favoriteId);
    Task<bool> RemoveFavoriteByItemAsync(int userId, int? celestialBodyId, int? articleId, int? discoveryId); // Este método faltaba
    Task<bool> IsFavoriteAsync(int userId, int? celestialBodyId, int? articleId, int? discoveryId);
}