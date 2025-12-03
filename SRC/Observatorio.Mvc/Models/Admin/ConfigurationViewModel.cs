namespace Observatorio.Mvc.Models.Admin;

public class ConfigurationViewModel
{
    public Dictionary<string, string> AppSettings { get; set; } = new();
}