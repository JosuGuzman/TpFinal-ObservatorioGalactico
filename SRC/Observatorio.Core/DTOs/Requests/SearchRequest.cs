namespace Observatorio.Core.DTOs.Requests;

public class SearchRequest
{
    public string Query { get; set; }
    public string Type { get; set; }
    public double? MinDistance { get; set; }
    public double? MaxDistance { get; set; }
    public string SpectralType { get; set; }
    public string PlanetType { get; set; }
    
    [Range(1, 100)]
    public int Page { get; set; } = 1;
    
    [Range(1, 100)]
    public int PageSize { get; set; } = 20;
    
    public string SortBy { get; set; } = "name";
    
    public bool SortDescending { get; set; } = false;
}