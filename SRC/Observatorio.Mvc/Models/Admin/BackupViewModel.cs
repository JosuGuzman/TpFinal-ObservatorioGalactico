namespace Observatorio.Mvc.Models.Admin;

public class BackupViewModel
{
    public bool IncludeDatabase { get; set; } = true;
    public bool IncludeUploads { get; set; } = true;
    public bool IncludeLogs { get; set; } = false;
    public string BackupType { get; set; } = "full";
}