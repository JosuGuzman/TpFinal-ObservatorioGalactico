namespace Observatorio.Core.Interfaces;

public interface IUserProfileService
{
    Task AddFavoriteAsync(int userId, string objectType, int objectId);
    Task RemoveFavoriteAsync(int userId, string objectType, int objectId);
    Task RemoveFavoriteByIdAsync(int favoriteId);
    Task<IEnumerable<UserFavorite>> GetUserFavoritesAsync(int userId);
    Task<bool> IsFavoriteAsync(int userId, string objectType, int objectId);
    Task<int> GetFavoriteCountAsync(string objectType, int objectId);
    
    Task AddToHistoryAsync(int userId, string objectType, int objectId, 
                          int? durationSeconds = null, string searchCriteria = null);
    Task<IEnumerable<ExplorationHistory>> GetUserHistoryAsync(int userId, int limit = 50);
    Task ClearHistoryAsync(int userId);
    
    Task<SavedSearch> SaveSearchAsync(int userId, string name, string criteria);
    Task<IEnumerable<SavedSearch>> GetUserSavedSearchesAsync(int userId);
    Task DeleteSavedSearchAsync(int searchId);
}