namespace Observatorio.Mvc.Models.Base;

public abstract class BaseSearchViewModel : BaseViewModel
{
    public string Query { get; set; }
    public string SortBy { get; set; }
    public bool SortDescending { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}