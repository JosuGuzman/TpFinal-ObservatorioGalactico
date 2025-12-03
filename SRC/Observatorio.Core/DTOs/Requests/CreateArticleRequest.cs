namespace Observatorio.Core.DTOs.Requests;

public class CreateArticleRequest
{
    [Required, StringLength(300)]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    public string Tags { get; set; }

    public string FeaturedImage { get; set; }

    public string State { get; set; } = "Draft";
}