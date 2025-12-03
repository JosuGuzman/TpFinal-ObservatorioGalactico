namespace Observatorio.Mvc.Models.Admin;

public class ApiKeysViewModel
{
    public List<ApiKeyViewModel> ApiKeys { get; set; } = new();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}