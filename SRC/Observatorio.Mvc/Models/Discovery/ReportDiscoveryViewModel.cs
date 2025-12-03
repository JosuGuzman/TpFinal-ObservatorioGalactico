using System.ComponentModel.DataAnnotations;

namespace Observatorio.Mvc.Models.Discovery;

public class ReportDiscoveryViewModel
{
    [Required(ErrorMessage = "El tipo de objeto es requerido")]
    [Display(Name = "Tipo de Objeto")]
    public string ObjectType { get; set; }
    
    [StringLength(250, ErrorMessage = "El nombre sugerido no puede exceder 250 caracteres")]
    [Display(Name = "Nombre Sugerido")]
    public string SuggestedName { get; set; }
    
    [Required(ErrorMessage = "La ascensión recta es requerida")]
    [Range(0, 360, ErrorMessage = "La ascensión recta debe estar entre 0 y 360 grados")]
    [Display(Name = "Ascensión Recta (0-360)")]
    public double RA { get; set; }
    
    [Required(ErrorMessage = "La declinación es requerida")]
    [Range(-90, 90, ErrorMessage = "La declinación debe estar entre -90 y 90 grados")]
    [Display(Name = "Declinación (-90 a 90)")]
    public double Dec { get; set; }
    
    [Required(ErrorMessage = "La descripción es requerida")]
    [StringLength(5000, ErrorMessage = "La descripción no puede exceder 5000 caracteres")]
    [Display(Name = "Descripción")]
    public string Description { get; set; }
    
    [Display(Name = "Archivos Adjuntos (JSON)")]
    public string Attachments { get; set; }
}