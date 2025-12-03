using System.ComponentModel.DataAnnotations;

namespace Observatorio.Mvc.Models.Article;

public class CreateArticleViewModel
{
    [Required(ErrorMessage = "El título es requerido")]
    [StringLength(300, ErrorMessage = "El título no puede exceder 300 caracteres")]
    [Display(Name = "Título")]
    public string Title { get; set; }
    
    [Required(ErrorMessage = "El contenido es requerido")]
    [Display(Name = "Contenido")]
    public string Content { get; set; }
    
    [Display(Name = "Etiquetas (JSON)")]
    public string Tags { get; set; }
    
    [Display(Name = "Imagen Destacada")]
    public string FeaturedImage { get; set; }
}