namespace Observatorio.Core.Services;

public class UserProfileService : IUserProfileService
{
    private readonly IUserFavoriteRepository _favoriteRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILoggingService _loggingService;

    public UserProfileService(
        IUserFavoriteRepository favoriteRepository,
        IUserRepository userRepository,
        ILoggingService loggingService)
    {
        _favoriteRepository = favoriteRepository;
        _userRepository = userRepository;
        _loggingService = loggingService;
    }

    // Favoritos
    public async Task AddFavoriteAsync(int userId, string objectType, int objectId)
    {
        try
        {
            if (await _favoriteRepository.IsFavoritedAsync(userId, objectType, objectId))
                throw new ValidationException(ErrorMessages.ALREADY_FAVORITED);

            var favorite = new UserFavorite
            {
                UserID = userId,
                ObjectType = Enum.Parse<ObjectType>(objectType),
                ObjectID = objectId,
                CreatedAt = DateTime.UtcNow
            };

            await _favoriteRepository.AddAsync(favorite);
            
            await _loggingService.LogInfoAsync("FavoriteAdded", 
                $"User {userId} added {objectType} {objectId} to favorites", userId);
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync("FavoriteAdd", 
                $"Error adding favorite for user {userId}", userId, null, ex);
            throw;
        }
    }

    public async Task RemoveFavoriteAsync(int userId, string objectType, int objectId)
    {
        try
        {
            var favorite = await _favoriteRepository.GetByUserAndObjectAsync(userId, objectType, objectId);
            if (favorite == null)
                throw new NotFoundException("Favorite", $"{userId}-{objectType}-{objectId}");

            await _favoriteRepository.DeleteAsync(favorite.FavoriteID);
            
            await _loggingService.LogInfoAsync("FavoriteRemoved", 
                $"User {userId} removed {objectType} {objectId} from favorites", userId);
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync("FavoriteRemove", 
                $"Error removing favorite for user {userId}", userId, null, ex);
            throw;
        }
    }

    public async Task RemoveFavoriteByIdAsync(int favoriteId)
    {
        var favorite = await _favoriteRepository.GetByIdAsync(favoriteId);
        if (favorite == null)
            throw new NotFoundException("Favorite", favoriteId);

        await _favoriteRepository.DeleteAsync(favoriteId);
        
        await _loggingService.LogInfoAsync("FavoriteRemovedById", 
            $"Favorite {favoriteId} removed", favorite.UserID);
    }

    public async Task<IEnumerable<UserFavorite>> GetUserFavoritesAsync(int userId)
    {
        return await _favoriteRepository.GetByUserAsync(userId);
    }

    public async Task<bool> IsFavoriteAsync(int userId, string objectType, int objectId)
    {
        return await _favoriteRepository.IsFavoritedAsync(userId, objectType, objectId);
    }

    public async Task<int> GetFavoriteCountAsync(string objectType, int objectId)
    {
        return await _favoriteRepository.CountByObjectAsync(objectType, objectId);
    }

    // Historial
    public async Task AddToHistoryAsync(int userId, string objectType, int objectId, 
        int? durationSeconds = null, string searchCriteria = null)
    {
        try
        {
            var history = new ExplorationHistory
            {
                UserID = userId,
                ObjectType = Enum.Parse<ObjectType>(objectType),
                ObjectID = objectId,
                AccessedAt = DateTime.UtcNow,
                DurationSeconds = durationSeconds,
                SearchCriteria = searchCriteria
            };

            // En una implementación real, esto sería un repositorio separado
            await _loggingService.LogInfoAsync("HistoryAdded", 
                $"User {userId} viewed {objectType} {objectId}", userId);
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync("HistoryAdd", 
                $"Error adding to history for user {userId}", userId, null, ex);
            // No lanzamos excepción para no interrumpir la experiencia del usuario
        }
    }

    public async Task<IEnumerable<ExplorationHistory>> GetUserHistoryAsync(int userId, int limit = 50)
    {
        // En una implementación real, esto consultaría la base de datos
        // Por ahora devolvemos una lista vacía
        await _loggingService.LogInfoAsync("HistoryRetrieved", 
            $"History retrieved for user {userId}", userId);
        return new List<ExplorationHistory>();
    }

    public async Task ClearHistoryAsync(int userId)
    {
        // En una implementación real, esto borraría los registros de la base de datos
        await _loggingService.LogInfoAsync("HistoryCleared", 
            $"History cleared for user {userId}", userId);
    }

    // Búsquedas guardadas
    public async Task<SavedSearch> SaveSearchAsync(int userId, string name, string criteria)
    {
        try
        {
            var search = new SavedSearch
            {
                UserID = userId,
                Name = name,
                Criteria = criteria,
                CreatedAt = DateTime.UtcNow
            };

            // En una implementación real, esto sería un repositorio separado
            await _loggingService.LogInfoAsync("SearchSaved", 
                $"User {userId} saved search: {name}", userId);
            
            return search;
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync("SearchSave", 
                $"Error saving search for user {userId}", userId, null, ex);
            throw;
        }
    }

    public async Task<IEnumerable<SavedSearch>> GetUserSavedSearchesAsync(int userId)
    {
        // En una implementación real, esto consultaría la base de datos
        await _loggingService.LogInfoAsync("SearchesRetrieved", 
            $"Saved searches retrieved for user {userId}", userId);
        return new List<SavedSearch>();
    }

    public async Task DeleteSavedSearchAsync(int searchId)
    {
        // En una implementación real, esto borraría el registro de la base de datos
        await _loggingService.LogInfoAsync("SearchDeleted", 
            $"Saved search {searchId} deleted", null);
    }
}