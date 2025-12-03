namespace Observatorio.Mvc.Models.Admin;

public class UserManagementViewModel
{
    public List<UserViewModel> Users { get; set; } = new();
    public string Search { get; set; }
    public string SelectedRole { get; set; }
    public string SelectedStatus { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public string SortBy { get; set; }
    public bool SortDescending { get; set; }
    public List<RoleViewModel> AvailableRoles { get; set; } = new();
    public List<string> AvailableStatuses { get; set; } = new();
}