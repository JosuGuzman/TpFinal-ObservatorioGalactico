namespace WatchTower.Infrastructure.Services;

public class FavoriteService : IFavoriteService
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICelestialBodyRepository _celestialBodyRepository;
    private readonly IArticleRepository _articleRepository;
    private readonly IDiscoveryRepository _discoveryRepository;

    public FavoriteService(
        IFavoriteRepository favoriteRepository,
        IUserRepository userRepository,
        ICelestialBodyRepository celestialBodyRepository,
        IArticleRepository articleRepository,
        IDiscoveryRepository discoveryRepository)
    {
        _favoriteRepository = favoriteRepository;
        _userRepository = userRepository;
        _celestialBodyRepository = celestialBodyRepository;
        _articleRepository = articleRepository;
        _discoveryRepository = discoveryRepository;
    }

    public async Task<IEnumerable<FavoriteResponse>> GetUserFavoritesAsync(int userId)
    {
        var favorites = await _favoriteRepository.GetUserFavoritesAsync(userId);
        
        var response = new List<FavoriteResponse>();
        
        foreach (var favorite in favorites)
        {
            string itemName = "Unknown";
            
            if (favorite.CelestialBodyId.HasValue)
            {
                var body = await _celestialBodyRepository.GetByIdAsync(favorite.CelestialBodyId.Value);
                itemName = body?.Name ?? "Unknown Celestial Body";
            }
            else if (favorite.ArticleId.HasValue)
            {
                var article = await _articleRepository.GetByIdAsync(favorite.ArticleId.Value);
                itemName = article?.Title ?? "Unknown Article";
            }
            else if (favorite.DiscoveryId.HasValue)
            {
                var discovery = await _discoveryRepository.GetByIdAsync(favorite.DiscoveryId.Value);
                itemName = discovery?.Title ?? "Unknown Discovery";
            }

            response.Add(new FavoriteResponse
            {
                FavoriteId = favorite.FavoriteId,
                ItemType = favorite.CelestialBodyId != null ? "CelestialBody" :
                          favorite.ArticleId != null ? "Article" : "Discovery",
                ItemName = itemName,
                ItemId = favorite.CelestialBodyId ?? favorite.ArticleId ?? favorite.DiscoveryId,
                CreatedAt = favorite.CreatedAt
            });
        }
        
        return response;
    }

    public async Task<FavoriteResponse> AddFavoriteAsync(FavoriteCreateRequest request, int userId)
    {
        // Validar que el item existe
        if (request.CelestialBodyId.HasValue)
        {
            var body = await _celestialBodyRepository.GetByIdAsync(request.CelestialBodyId.Value);
            if (body == null) throw new NotFoundException("Celestial body", request.CelestialBodyId.Value);
        }
        else if (request.ArticleId.HasValue)
        {
            var article = await _articleRepository.GetByIdAsync(request.ArticleId.Value);
            if (article == null) throw new NotFoundException("Article", request.ArticleId.Value);
        }
        else if (request.DiscoveryId.HasValue)
        {
            var discovery = await _discoveryRepository.GetByIdAsync(request.DiscoveryId.Value);
            if (discovery == null) throw new NotFoundException("Discovery", request.DiscoveryId.Value);
        }
        else
        {
            throw new BusinessRuleException("At least one item ID must be provided");
        }

        var favorite = new Favorite
        {
            UserId = userId,
            CelestialBodyId = request.CelestialBodyId,
            ArticleId = request.ArticleId,
            DiscoveryId = request.DiscoveryId
        };

        var success = await _favoriteRepository.AddFavoriteAsync(favorite);
        if (!success)
            throw new BusinessRuleException("Item is already in favorites");

        // Obtener el favorito recién creado
        var favorites = await _favoriteRepository.GetUserFavoritesAsync(userId);
        var newFavorite = favorites.First(f =>
            f.CelestialBodyId == request.CelestialBodyId &&
            f.ArticleId == request.ArticleId &&
            f.DiscoveryId == request.DiscoveryId);

        string itemName = await GetItemNameAsync(newFavorite);

        return new FavoriteResponse
        {
            FavoriteId = newFavorite.FavoriteId,
            ItemType = newFavorite.CelestialBodyId != null ? "CelestialBody" :
                      newFavorite.ArticleId != null ? "Article" : "Discovery",
            ItemName = itemName,
            ItemId = newFavorite.CelestialBodyId ?? newFavorite.ArticleId ?? newFavorite.DiscoveryId,
            CreatedAt = newFavorite.CreatedAt
        };
    }

    public async Task<bool> RemoveFavoriteAsync(int favoriteId, int userId)
    {
        var favorite = await _favoriteRepository.GetByIdAsync(favoriteId);
        if (favorite == null) throw new NotFoundException("Favorite", favoriteId);

        if (favorite.UserId != userId)
            throw new ForbiddenException("You are not allowed to remove this favorite");

        return await _favoriteRepository.RemoveFavoriteAsync(favoriteId);
    }

    public async Task<bool> RemoveFavoriteByItemAsync(FavoriteCreateRequest request, int userId)
    {
        return await _favoriteRepository.RemoveFavoriteByItemAsync(
            userId, request.CelestialBodyId, request.ArticleId, request.DiscoveryId);
    }

    public async Task<bool> IsFavoriteAsync(int userId, int? celestialBodyId, int? articleId, int? discoveryId)
    {
        return await _favoriteRepository.IsFavoriteAsync(userId, celestialBodyId, articleId, discoveryId);
    }

    private async Task<string> GetItemNameAsync(Favorite favorite)
    {
        if (favorite.CelestialBodyId.HasValue)
        {
            var body = await _celestialBodyRepository.GetByIdAsync(favorite.CelestialBodyId.Value);
            return body?.Name ?? "Unknown Celestial Body";
        }
        else if (favorite.ArticleId.HasValue)
        {
            var article = await _articleRepository.GetByIdAsync(favorite.ArticleId.Value);
            return article?.Title ?? "Unknown Article";
        }
        else if (favorite.DiscoveryId.HasValue)
        {
            var discovery = await _discoveryRepository.GetByIdAsync(favorite.DiscoveryId.Value);
            return discovery?.Title ?? "Unknown Discovery";
        }
        
        return "Unknown";
    }
}