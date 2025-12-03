namespace Observatorio.Mvc.Models.Admin;

public class BulkActionsViewModel
{
    public List<BulkActionViewModel> AvailableActions { get; set; } = new();
    public List<int> SelectedUserIds { get; set; } = new();
}