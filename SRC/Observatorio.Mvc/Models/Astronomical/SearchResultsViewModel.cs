namespace Observatorio.Mvc.Models.Astronomical;

public class SearchResultsViewModel
{
    public List<SearchResultViewModel> Results { get; set; } = new();
    public string Query { get; set; }
    public string Type { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public string SortBy { get; set; }
    public bool SortDescending { get; set; }
}