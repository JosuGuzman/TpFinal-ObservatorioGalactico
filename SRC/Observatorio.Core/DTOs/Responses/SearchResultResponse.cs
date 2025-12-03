namespace Observatorio.Core.DTOs.Responses;

public class SearchResultResponse
{
    public string Type { get; set; }
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double? Distance { get; set; }
    public double? Habitability { get; set; }
    public DateTime? CreatedAt { get; set; }
    public double Relevance { get; set; }
}