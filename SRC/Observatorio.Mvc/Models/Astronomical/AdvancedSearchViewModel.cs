using System.ComponentModel.DataAnnotations;

namespace Observatorio.Mvc.Models.Astronomical;

public class AdvancedSearchViewModel
{
    // Opciones de búsqueda
    public bool SearchGalaxies { get; set; } = true;
    public bool SearchStars { get; set; } = true;
    public bool SearchPlanets { get; set; } = true;
    public bool SearchDiscoveries { get; set; } = false;
    
    // Filtros de galaxias
    public string GalaxyType { get; set; }
    public List<string> GalaxyTypes { get; set; } = new();
    
    [Range(0, double.MaxValue, ErrorMessage = "La distancia mínima debe ser positiva")]
    public double? MinDistance { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "La distancia máxima debe ser positiva")]
    public double? MaxDistance { get; set; }
    
    [Range(-30, 30, ErrorMessage = "La magnitud debe estar entre -30 y 30")]
    public float? MinMagnitude { get; set; }
    
    [Range(-30, 30, ErrorMessage = "La magnitud debe estar entre -30 y 30")]
    public float? MaxMagnitude { get; set; }
    
    // Filtros de estrellas
    public string SpectralType { get; set; }
    public List<string> SpectralTypes { get; set; } = new();
    
    [Range(0, 50000, ErrorMessage = "La temperatura debe estar entre 0 y 50000 K")]
    public int? MinTemp { get; set; }
    
    [Range(0, 50000, ErrorMessage = "La temperatura debe estar entre 0 y 50000 K")]
    public int? MaxTemp { get; set; }
    
    [Range(0, 100, ErrorMessage = "La masa debe estar entre 0 y 100 masas solares")]
    public double? MinMass { get; set; }
    
    [Range(0, 100, ErrorMessage = "La masa debe estar entre 0 y 100 masas solares")]
    public double? MaxMass { get; set; }
    
    // Filtros de planetas
    public string PlanetType { get; set; }
    public List<string> PlanetTypes { get; set; } = new();
    
    [Range(0, 1, ErrorMessage = "El índice de habitabilidad debe estar entre 0 y 1")]
    public double? MinHabitability { get; set; }
    
    [Range(0, 1, ErrorMessage = "El índice de habitabilidad debe estar entre 0 y 1")]
    public double? MaxHabitability { get; set; }
    
    [Range(0, 100, ErrorMessage = "La distancia orbital debe estar entre 0 y 100 AU")]
    public double? MinOrbitalDistance { get; set; }
    
    [Range(0, 100, ErrorMessage = "La distancia orbital debe estar entre 0 y 100 AU")]
    public double? MaxOrbitalDistance { get; set; }
    
    // Filtros de descubrimientos
    public string DiscoveryState { get; set; }
    public List<string> DiscoveryStates { get; set; } = new();
    
    [Range(0, int.MaxValue, ErrorMessage = "El mínimo de votos debe ser positivo")]
    public int? MinVotes { get; set; }
    
    [Range(0, 1, ErrorMessage = "La tasa de aprobación debe estar entre 0 y 1")]
    public double? MinApprovalRate { get; set; }
    
    // Ordenamiento
    public string SortBy { get; set; } = "name";
    public bool SortDescending { get; set; } = false;
    
    // Resultados
    public SearchResultsViewModel Results { get; set; }
}