namespace Observatorio.Mvc.Models.Shared;

public class BreadcrumbViewModel
{
    public List<BreadcrumbItemViewModel> Items { get; set; } = new();
}

public class BreadcrumbItemViewModel
{
    public string Text { get; set; }
    public string Url { get; set; }
    public bool IsActive { get; set; }
}