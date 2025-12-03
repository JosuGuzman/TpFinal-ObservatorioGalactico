using System.ComponentModel.DataAnnotations;

namespace Observatorio.Mvc.Models.Account;

public class EditProfileViewModel
{
    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 100 caracteres")]
    [Display(Name = "Nombre de Usuario")]
    public string UserName { get; set; }
}