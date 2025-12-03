using System.ComponentModel.DataAnnotations;

namespace Observatorio.Mvc.Models.Astronomical;

public class SearchViewModel
{
    public string Query { get; set; }
    public string Type { get; set; } = "all"; // all, galaxies, stars, planets, articles, events, discoveries
    
    // Filtros generales
    [Range(0, double.MaxValue, ErrorMessage = "La distancia mínima debe ser positiva")]
    public double? MinDistance { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "La distancia máxima debe ser positiva")]
    public double? MaxDistance { get; set; }
    
    [Range(0, 1, ErrorMessage = "El índice de habitabilidad debe estar entre 0 y 1")]
    public double? MinHabitability { get; set; }
    
    [Range(0, 1, ErrorMessage = "El índice de habitabilidad debe estar entre 0 y 1")]
    public double? MaxHabitability { get; set; }
    
    // Filtros específicos por tipo
    public string GalaxyType { get; set; }
    public string SpectralType { get; set; }
    public string PlanetType { get; set; }
    
    // Paginación y ordenamiento
    [Range(1, 100, ErrorMessage = "La página debe estar entre 1 y 100")]
    public int Page { get; set; } = 1;
    
    [Range(1, 100, ErrorMessage = "El tamaño de página debe estar entre 1 y 100")]
    public int PageSize { get; set; } = 20;
    
    public string SortBy { get; set; } = "relevance";
    public bool SortDescending { get; set; } = true;
    
    // Resultados
    public SearchResultsViewModel Results { get; set; }
}