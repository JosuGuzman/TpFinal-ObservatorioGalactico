using System.ComponentModel.DataAnnotations;

namespace Observatorio.Mvc.Models.Shared;

public class FileUploadViewModel
{
    [Required(ErrorMessage = "Por favor seleccione un archivo")]
    [Display(Name = "Archivo")]
    public IFormFile File { get; set; }
    
    [Display(Name = "Descripción")]
    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string Description { get; set; }
    
    [Display(Name = "Tipo de archivo")]
    public string FileType { get; set; } = "image"; // image, document, data
    
    public List<string> AllowedExtensions { get; set; } = new() { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".txt", ".csv", ".json" };
    public long MaxFileSize { get; set; } = 10 * 1024 * 1024; // 10 MB
}