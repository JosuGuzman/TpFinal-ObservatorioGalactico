// DTOs/CommentDtos.cs
namespace WatchTower.Core.DTOs;

public class CommentCreateRequest
{
    public string Content { get; set; } = string.Empty;
    public int? DiscoveryId { get; set; }
    public int? ArticleId { get; set; }
    public int? ParentCommentId { get; set; }
}

public class CommentUpdateRequest
{
    public string Content { get; set; } = string.Empty;
}

public class CommentResponse
{
    public int CommentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public IEnumerable<CommentResponse> Replies { get; set; } = new List<CommentResponse>();
}