namespace WatchTower.Infrastructure.Services;

public class CelestialBodyService : ICelestialBodyService
{
    private readonly ICelestialBodyRepository _celestialBodyRepository;
    private readonly IUserRepository _userRepository;

    public CelestialBodyService(ICelestialBodyRepository celestialBodyRepository, IUserRepository userRepository)
    {
        _celestialBodyRepository = celestialBodyRepository;
        _userRepository = userRepository;
    }

    public async Task<CelestialBodyDetailResponse?> GetByIdAsync(int id)
    {
        var body = await _celestialBodyRepository.GetByIdAsync(id);
        if (body == null) return null;

        var creator = body.CreatedBy.HasValue ? await _userRepository.GetByIdAsync(body.CreatedBy.Value) : null;

        return new CelestialBodyDetailResponse
        {
            BodyId = body.BodyId,
            Name = body.Name,
            Type = body.Type.ToString(),
            SubType = body.SubType,
            Constellation = body.Constellation,
            RightAscension = body.RightAscension,
            Declination = body.Declination,
            Distance = body.Distance,
            ApparentMagnitude = body.ApparentMagnitude,
            AbsoluteMagnitude = body.AbsoluteMagnitude,
            Mass = body.Mass,
            Radius = body.Radius,
            Temperature = body.Temperature,
            Description = body.Description,
            DiscoveryDate = body.DiscoveryDate,
            NASA_ImageURL = body.NASA_ImageURL,
            IsVerified = body.IsVerified,
            CreatedAt = body.CreatedAt,
            CreatedBy = creator?.Username
        };
    }

    public async Task<PagedResult<CelestialBodyResponse>> SearchAsync(CelestialBodySearchRequest request)
    {
        var bodies = await _celestialBodyRepository.SearchAsync(request);
        var totalCount = await _celestialBodyRepository.GetTotalCountAsync();

        var response = bodies.Select(body => new CelestialBodyResponse
        {
            BodyId = body.BodyId,
            Name = body.Name,
            Type = body.Type.ToString(),
            SubType = body.SubType,
            Constellation = body.Constellation,
            Distance = body.Distance,
            ApparentMagnitude = body.ApparentMagnitude,
            Description = body.Description,
            NASA_ImageURL = body.NASA_ImageURL,
            IsVerified = body.IsVerified,
            CreatedAt = body.CreatedAt
        });

        return new PagedResult<CelestialBodyResponse>
        {
            Items = response,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<CelestialBodyResponse> CreateAsync(CelestialBodyCreateRequest request, int createdBy)
    {
        var body = new CelestialBody
        {
            Name = request.Name,
            Type = request.Type,
            SubType = request.SubType,
            Constellation = request.Constellation,
            RightAscension = request.RightAscension,
            Declination = request.Declination,
            Distance = request.Distance,
            ApparentMagnitude = request.ApparentMagnitude,
            AbsoluteMagnitude = request.AbsoluteMagnitude,
            Mass = request.Mass,
            Radius = request.Radius,
            Temperature = request.Temperature,
            Description = request.Description,
            DiscoveryDate = request.DiscoveryDate,
            NASA_ImageURL = request.NASA_ImageURL,
            IsVerified = false,
            CreatedBy = createdBy
        };

        var bodyId = await _celestialBodyRepository.CreateAsync(body);
        body.BodyId = bodyId;

        return new CelestialBodyResponse
        {
            BodyId = body.BodyId,
            Name = body.Name,
            Type = body.Type.ToString(),
            SubType = body.SubType,
            Constellation = body.Constellation,
            Distance = body.Distance,
            ApparentMagnitude = body.ApparentMagnitude,
            Description = body.Description,
            NASA_ImageURL = body.NASA_ImageURL,
            IsVerified = body.IsVerified,
            CreatedAt = body.CreatedAt
        };
    }

    public async Task<CelestialBodyResponse?> UpdateAsync(int id, CelestialBodyUpdateRequest request)
    {
        var body = await _celestialBodyRepository.GetByIdAsync(id);
        if (body == null) return null;

        if (!string.IsNullOrEmpty(request.Name)) body.Name = request.Name;
        if (request.Type.HasValue) body.Type = request.Type.Value;
        if (request.SubType != null) body.SubType = request.SubType;
        if (request.Constellation != null) body.Constellation = request.Constellation;
        if (request.RightAscension != null) body.RightAscension = request.RightAscension;
        if (request.Declination != null) body.Declination = request.Declination;
        if (request.Distance.HasValue) body.Distance = request.Distance.Value;
        if (request.ApparentMagnitude.HasValue) body.ApparentMagnitude = request.ApparentMagnitude.Value;
        if (request.AbsoluteMagnitude.HasValue) body.AbsoluteMagnitude = request.AbsoluteMagnitude.Value;
        if (request.Mass.HasValue) body.Mass = request.Mass.Value;
        if (request.Radius.HasValue) body.Radius = request.Radius.Value;
        if (request.Temperature.HasValue) body.Temperature = request.Temperature.Value;
        if (request.Description != null) body.Description = request.Description;
        if (request.DiscoveryDate.HasValue) body.DiscoveryDate = request.DiscoveryDate.Value;
        if (request.NASA_ImageURL != null) body.NASA_ImageURL = request.NASA_ImageURL;
        if (request.IsVerified.HasValue) body.IsVerified = request.IsVerified.Value;

        await _celestialBodyRepository.UpdateAsync(body);

        return new CelestialBodyResponse
        {
            BodyId = body.BodyId,
            Name = body.Name,
            Type = body.Type.ToString(),
            SubType = body.SubType,
            Constellation = body.Constellation,
            Distance = body.Distance,
            ApparentMagnitude = body.ApparentMagnitude,
            Description = body.Description,
            NASA_ImageURL = body.NASA_ImageURL,
            IsVerified = body.IsVerified,
            CreatedAt = body.CreatedAt
        };
    }

    public async Task<bool> VerifyAsync(int id, int verifiedBy)
    {
        return await _celestialBodyRepository.VerifyBodyAsync(id, verifiedBy);
    }

    public async Task<IEnumerable<CelestialBodyResponse>> GetRecentAsync(int count)
    {
        var bodies = await _celestialBodyRepository.GetRecentBodiesAsync(count);
        return bodies.Select(body => new CelestialBodyResponse
        {
            BodyId = body.BodyId,
            Name = body.Name,
            Type = body.Type.ToString(),
            SubType = body.SubType,
            Constellation = body.Constellation,
            Distance = body.Distance,
            ApparentMagnitude = body.ApparentMagnitude,
            Description = body.Description,
            NASA_ImageURL = body.NASA_ImageURL,
            IsVerified = body.IsVerified,
            CreatedAt = body.CreatedAt
        });
    }
}
