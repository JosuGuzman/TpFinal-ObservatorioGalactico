namespace Observatorio.Mvc.Models.Base;

public abstract class BaseViewModel
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string PageTitle { get; set; }
    public bool HasErrors { get; set; }
    public List<string> ErrorMessages { get; set; } = new();
    public List<string> SuccessMessages { get; set; } = new();
    public List<string> WarningMessages { get; set; } = new();
    public List<string> InfoMessages { get; set; } = new();
}