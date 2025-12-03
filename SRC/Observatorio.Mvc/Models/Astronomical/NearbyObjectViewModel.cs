namespace Observatorio.Mvc.Models.Astronomical;

public class NearbyObjectViewModel
{
    public string Type { get; set; }
    public int ID { get; set; }
    public string Name { get; set; }
    public double RA { get; set; }
    public double Dec { get; set; }
    public double Distance { get; set; }
}