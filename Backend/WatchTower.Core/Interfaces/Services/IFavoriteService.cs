namespace WatchTower.Core.Interfaces.Services;

public interface IFavoriteService
{
    Task<IEnumerable<FavoriteResponse>> GetUserFavoritesAsync(int userId);
    Task<FavoriteResponse> AddFavoriteAsync(FavoriteCreateRequest request, int userId);
    Task<bool> RemoveFavoriteAsync(int favoriteId, int userId);
    Task<bool> RemoveFavoriteByItemAsync(FavoriteCreateRequest request, int userId);
    Task<bool> IsFavoriteAsync(int userId, int? celestialBodyId, int? articleId, int? discoveryId);
}