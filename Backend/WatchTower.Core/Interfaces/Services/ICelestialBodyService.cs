namespace WatchTower.Core.Interfaces.Services;

public interface ICelestialBodyService
{
    Task<CelestialBodyDetailResponse?> GetByIdAsync(int id);
    Task<PagedResult<CelestialBodyResponse>> SearchAsync(CelestialBodySearchRequest request);
    Task<CelestialBodyResponse> CreateAsync(CelestialBodyCreateRequest request, int createdBy);
    Task<CelestialBodyResponse?> UpdateAsync(int id, CelestialBodyUpdateRequest request);
    Task<bool> VerifyAsync(int id, int verifiedBy);
}