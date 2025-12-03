using System.ComponentModel.DataAnnotations;

namespace Observatorio.Mvc.Models.Admin;

public class EditUserViewModel
{
    public int UserID { get; set; }
    
    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 100 caracteres")]
    [Display(Name = "Nombre de Usuario")]
    public string UserName { get; set; }
    
    [Required(ErrorMessage = "El correo electrónico es requerido")]
    [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido")]
    [Display(Name = "Correo Electrónico")]
    public string Email { get; set; }
    
    [Display(Name = "Activo")]
    public bool IsActive { get; set; }
    
    [Required(ErrorMessage = "El rol es requerido")]
    [Display(Name = "Rol")]
    public int RoleID { get; set; }
    
    public List<RoleViewModel> AvailableRoles { get; set; } = new();
}