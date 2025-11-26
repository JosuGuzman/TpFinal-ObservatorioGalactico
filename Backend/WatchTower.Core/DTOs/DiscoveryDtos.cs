// DTOs/DiscoveryDtos.cs
namespace WatchTower.Core.DTOs;

public class DiscoverySearchRequest
{
    public string? SearchTerm { get; set; }
    public string? Status { get; set; }
    public int? MinRating { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
}

public class DiscoveryCreateRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Coordinates { get; set; }
    public int? CelestialBodyId { get; set; }
}

public class DiscoveryUpdateRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Coordinates { get; set; }
    public int? CelestialBodyId { get; set; }
    public DiscoveryStatus? Status { get; set; }
}

public class DiscoveryResponse
{
    public int DiscoveryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string ReportedBy { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? CelestialBodyName { get; set; }
    public int CommentCount { get; set; }
}

public class DiscoveryDetailResponse : DiscoveryResponse
{
    public string? Coordinates { get; set; }
    public DateTime? DiscoveryDate { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? VerifiedBy { get; set; }
    public string? NASA_API_Data { get; set; }
    public IEnumerable<CommentResponse> Comments { get; set; } = new List<CommentResponse>();
    public IEnumerable<VoteResponse> Votes { get; set; } = new List<VoteResponse>();
}

public class VoteResponse
{
    public int VoteId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public VoteType VoteType { get; set; }
    public DateTime CreatedAt { get; set; }
}