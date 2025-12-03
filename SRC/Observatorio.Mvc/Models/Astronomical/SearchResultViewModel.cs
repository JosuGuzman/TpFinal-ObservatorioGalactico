namespace Observatorio.Mvc.Models.Astronomical;

public class SearchResultViewModel
{
    public string Type { get; set; } // Galaxy, Star, Planet, Article, Event, Discovery
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double? Distance { get; set; }
    public double? Habitability { get; set; }
    public DateTime? CreatedAt { get; set; }
    public float Relevance { get; set; }
    public string ImageUrl { get; set; }
}