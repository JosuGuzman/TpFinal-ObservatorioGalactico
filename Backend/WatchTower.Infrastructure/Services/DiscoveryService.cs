namespace WatchTower.Infrastructure.Services;

public class DiscoveryService : IDiscoveryService
{
    private readonly IDiscoveryRepository _discoveryRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICelestialBodyRepository _celestialBodyRepository;

    public DiscoveryService(IDiscoveryRepository discoveryRepository, IUserRepository userRepository, ICelestialBodyRepository celestialBodyRepository)
    {
        _discoveryRepository = discoveryRepository;
        _userRepository = userRepository;
        _celestialBodyRepository = celestialBodyRepository;
    }

    public async Task<DiscoveryDetailResponse?> GetByIdAsync(int id)
    {
        var discovery = await _discoveryRepository.GetByIdAsync(id);
        if (discovery == null) return null;

        var rating = await _discoveryRepository.GetDiscoveryRatingAsync(id);

        return new DiscoveryDetailResponse
        {
            DiscoveryId = discovery.DiscoveryId,
            Title = discovery.Title,
            Description = discovery.Description,
            Status = discovery.Status.ToString(),
            CreatedAt = discovery.CreatedAt,
            ReportedBy = discovery.ReportedBy.ToString(),
            Rating = rating,
            CelestialBodyName = discovery.CelestialBodyName,
            Coordinates = discovery.Coordinates,
            DiscoveryDate = discovery.DiscoveryDate,
            VerifiedAt = discovery.VerifiedAt,
            VerifiedBy = discovery.VerifiedBy?.ToString(),
            NASA_API_Data = discovery.NASA_API_Data,
            Comments = new List<CommentResponse>(),
            Votes = new List<VoteResponse>()
        };
    }

    public async Task<PagedResult<DiscoveryResponse>> SearchAsync(DiscoverySearchRequest request)
    {
        var discoveries = await _discoveryRepository.SearchAsync(request);
        var totalCount = await _discoveryRepository.GetDiscoveryCountAsync();

        var response = discoveries.Select(discovery => new DiscoveryResponse
        {
            DiscoveryId = discovery.DiscoveryId,
            Title = discovery.Title,
            Description = discovery.Description,
            Status = discovery.Status.ToString(),
            CreatedAt = discovery.CreatedAt,
            ReportedBy = discovery.ReportedBy.ToString(),
            Rating = discovery.Rating,
            CelestialBodyName = discovery.CelestialBodyName,
            CommentCount = 0 // Se calcularía por separado
        });

        return new PagedResult<DiscoveryResponse>
        {
            Items = response,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<DiscoveryResponse> CreateAsync(DiscoveryCreateRequest request, int reportedBy)
    {
        var discovery = new Discovery
        {
            Title = request.Title,
            Description = request.Description,
            Coordinates = request.Coordinates,
            CelestialBodyId = request.CelestialBodyId,
            ReportedBy = reportedBy,
            Status = DiscoveryStatus.Pending
        };

        var discoveryId = await _discoveryRepository.CreateAsync(discovery);
        discovery.DiscoveryId = discoveryId;

        var reporter = await _userRepository.GetByIdAsync(reportedBy);

        return new DiscoveryResponse
        {
            DiscoveryId = discovery.DiscoveryId,
            Title = discovery.Title,
            Description = discovery.Description,
            Status = discovery.Status.ToString(),
            CreatedAt = discovery.CreatedAt,
            ReportedBy = reporter?.Username ?? "Unknown",
            Rating = 0,
            CelestialBodyName = null,
            CommentCount = 0
        };
    }

    public async Task<DiscoveryResponse?> UpdateAsync(int id, DiscoveryUpdateRequest request, int userId)
    {
        var discovery = await _discoveryRepository.GetByIdAsync(id);
        if (discovery == null) return null;

        // Solo el usuario que reportó el descubrimiento o un administrador/astrónomo puede actualizarlo
        if (discovery.ReportedBy != userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || (user.Role != UserRole.Admin && user.Role != UserRole.Astronomer))
                throw new ForbiddenException("You are not allowed to update this discovery");
        }

        if (!string.IsNullOrEmpty(request.Title)) discovery.Title = request.Title;
        if (!string.IsNullOrEmpty(request.Description)) discovery.Description = request.Description;
        if (request.Coordinates != null) discovery.Coordinates = request.Coordinates;
        if (request.CelestialBodyId.HasValue) discovery.CelestialBodyId = request.CelestialBodyId.Value;
        if (request.Status.HasValue) discovery.Status = request.Status.Value;

        await _discoveryRepository.UpdateAsync(discovery);

        var reporter = await _userRepository.GetByIdAsync(discovery.ReportedBy);
        var rating = await _discoveryRepository.GetDiscoveryRatingAsync(id);

        return new DiscoveryResponse
        {
            DiscoveryId = discovery.DiscoveryId,
            Title = discovery.Title,
            Description = discovery.Description,
            Status = discovery.Status.ToString(),
            CreatedAt = discovery.CreatedAt,
            ReportedBy = reporter?.Username ?? "Unknown",
            Rating = rating,
            CelestialBodyName = null,
            CommentCount = 0
        };
    }

    public async Task<bool> VoteAsync(int discoveryId, int userId, VoteType voteType)
    {
        var discovery = await _discoveryRepository.GetByIdAsync(discoveryId);
        if (discovery == null) throw new NotFoundException("Discovery", discoveryId);

        if (discovery.ReportedBy == userId)
            throw new BusinessRuleException("You cannot vote on your own discovery");

        return await _discoveryRepository.AddVoteAsync(discoveryId, userId, voteType);
    }

    public async Task<bool> UpdateStatusAsync(int discoveryId, DiscoveryStatus status, int verifiedBy)
    {
        var user = await _userRepository.GetByIdAsync(verifiedBy);
        if (user == null || (user.Role != UserRole.Admin && user.Role != UserRole.Astronomer))
            throw new ForbiddenException("Only administrators and astronomers can verify discoveries");

        return await _discoveryRepository.UpdateStatusAsync(discoveryId, status, verifiedBy);
    }

    public async Task<IEnumerable<DiscoveryResponse>> GetUserDiscoveriesAsync(int userId)
    {
        var discoveries = await _discoveryRepository.GetUserDiscoveriesAsync(userId);
        return discoveries.Select(discovery => new DiscoveryResponse
        {
            DiscoveryId = discovery.DiscoveryId,
            Title = discovery.Title,
            Description = discovery.Description,
            Status = discovery.Status.ToString(),
            CreatedAt = discovery.CreatedAt,
            ReportedBy = discovery.ReportedBy.ToString(),
            Rating = discovery.Rating,
            CelestialBodyName = null,
            CommentCount = 0
        });
    }
}