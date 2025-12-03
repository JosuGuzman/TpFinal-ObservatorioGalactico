using System.ComponentModel.DataAnnotations;

namespace Observatorio.Mvc.Models.Event;

public class EditEventViewModel
{
    public int EventID { get; set; }
    
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(250, ErrorMessage = "El nombre no puede exceder 250 caracteres")]
    [Display(Name = "Nombre")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "El tipo es requerido")]
    [Display(Name = "Tipo")]
    public string Type { get; set; }
    
    [Required(ErrorMessage = "La fecha y hora son requeridas")]
    [Display(Name = "Fecha y Hora")]
    public DateTime EventDate { get; set; }
    
    [Display(Name = "Descripción")]
    public string Description { get; set; }
    
    [Display(Name = "Visibilidad (JSON)")]
    public string Visibility { get; set; }
    
    [Display(Name = "Coordenadas (JSON)")]
    public string Coordinates { get; set; }
    
    [Range(1, 10080, ErrorMessage = "La duración debe estar entre 1 y 10080 minutos")]
    [Display(Name = "Duración (minutos)")]
    public int? DurationMinutes { get; set; }
    
    [Display(Name = "Recursos (JSON)")]
    public string Resources { get; set; }
}