using System.ComponentModel.DataAnnotations;

namespace Observatorio.Mvc.Models.Astronomical;

public class PlanetViewModel
{
    public int PlanetID { get; set; }
    
    [Required(ErrorMessage = "La estrella es requerida")]
    public int StarID { get; set; }
    public string StarName { get; set; }
    
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "El tipo es requerido")]
    public string PlanetType { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "La masa debe ser positiva")]
    public double? MassEarth { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "El radio debe ser positivo")]
    public double? RadiusEarth { get; set; }
    
    public double? Gravity { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "El período orbital debe ser positivo")]
    public double? OrbitalPeriodDays { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "La distancia orbital debe ser positiva")]
    public double? OrbitalDistanceAU { get; set; }
    
    [Range(0, 1, ErrorMessage = "La excentricidad debe estar entre 0 y 1")]
    public double? Eccentricity { get; set; }
    
    [Range(0, 1, ErrorMessage = "El índice de habitabilidad debe estar entre 0 y 1")]
    public double? HabitabilityScore { get; set; }
    
    public bool IsPotentiallyHabitable { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "La temperatura debe ser positiva")]
    public double? SurfaceTempK { get; set; }
    
    public DateTime? DiscoveryDate { get; set; }
    public string Atmosphere { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}