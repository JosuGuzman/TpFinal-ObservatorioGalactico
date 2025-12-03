namespace Observatorio.Mvc.Models.Discovery;

public class DiscoveryIndexViewModel
{
    public List<DiscoveryViewModel> Discoveries { get; set; } = new();
    public string State { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}