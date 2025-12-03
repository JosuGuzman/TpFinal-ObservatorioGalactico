using System.ComponentModel.DataAnnotations;

namespace Observatorio.Mvc.Models.Astronomical;

public class GalaxyViewModel
{
    public int GalaxyID { get; set; }
    
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "El tipo es requerido")]
    public string Type { get; set; }
    
    [Required(ErrorMessage = "La distancia es requerida")]
    [Range(0, double.MaxValue, ErrorMessage = "La distancia debe ser positiva")]
    public double DistanceLy { get; set; }
    
    [Range(-30, 30, ErrorMessage = "La magnitud aparente debe estar entre -30 y 30")]
    public float? ApparentMagnitude { get; set; }
    
    public double? AbsoluteMagnitude { get; set; }
    
    [Required(ErrorMessage = "La ascensión recta es requerida")]
    [Range(0, 360, ErrorMessage = "La ascensión recta debe estar entre 0 y 360 grados")]
    public double RA { get; set; }
    
    [Required(ErrorMessage = "La declinación es requerida")]
    [Range(-90, 90, ErrorMessage = "La declinación debe estar entre -90 y 90 grados")]
    public double Dec { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "El redshift debe ser positivo")]
    public double? Redshift { get; set; }
    
    public string Description { get; set; }
    public string ImageURLs { get; set; }
    public string Discoverer { get; set; }
    
    [Range(1000, 9999, ErrorMessage = "El año de descubrimiento debe estar entre 1000 y 9999")]
    public int? YearDiscovery { get; set; }
    
    public string References { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int StarCount { get; set; }
}