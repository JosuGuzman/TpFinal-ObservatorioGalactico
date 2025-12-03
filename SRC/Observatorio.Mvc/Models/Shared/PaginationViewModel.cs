namespace Observatorio.Mvc.Models.Shared;

public class PaginationViewModel
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
    public int FirstItem => (CurrentPage - 1) * PageSize + 1;
    public int LastItem => Math.Min(CurrentPage * PageSize, TotalCount);
}