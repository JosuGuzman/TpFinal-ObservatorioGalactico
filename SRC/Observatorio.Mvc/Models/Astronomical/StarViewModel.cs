using System.ComponentModel.DataAnnotations;

namespace Observatorio.Mvc.Models.Astronomical;

public class StarViewModel
{
    public int StarID { get; set; }
    public int? GalaxyID { get; set; }
    public string GalaxyName { get; set; }
    
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
    public string Name { get; set; }
    
    public string SpectralType { get; set; }
    public string Color { get; set; }
    
    [Range(0, 50000, ErrorMessage = "La temperatura debe estar entre 0 y 50000 K")]
    public int? SurfaceTempK { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "La masa debe ser positiva")]
    public double? MassSolar { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "El radio debe ser positivo")]
    public double? RadiusSolar { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "La luminosidad debe ser positiva")]
    public double? LuminositySolar { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "La distancia debe ser positiva")]
    public double? DistanceLy { get; set; }
    
    [Required(ErrorMessage = "La ascensión recta es requerida")]
    [Range(0, 360, ErrorMessage = "La ascensión recta debe estar entre 0 y 360 grados")]
    public double RA { get; set; }
    
    [Required(ErrorMessage = "La declinación es requerida")]
    [Range(-90, 90, ErrorMessage = "La declinación debe estar entre -90 y 90 grados")]
    public double Dec { get; set; }
    
    public double? RadialVelocity { get; set; }
    
    [Range(-30, 30, ErrorMessage = "La magnitud aparente debe estar entre -30 y 30")]
    public float? ApparentMagnitude { get; set; }
    
    public double? AbsoluteMagnitude { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "La edad estimada debe ser positiva")]
    public double? EstimatedAgeMillionYears { get; set; }
    
    public int PlanetCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}