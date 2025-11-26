namespace WatchTower.Core.DTOs;

public class CommentResponse
{
    public int CommentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public IEnumerable<CommentResponse> Replies { get; set; } = new List<CommentResponse>();
}