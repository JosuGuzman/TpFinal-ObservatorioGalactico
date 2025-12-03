using System.ComponentModel.DataAnnotations;

namespace Observatorio.Mvc.Models.Account;

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "La contraseña actual es requerida")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña Actual")]
    public string CurrentPassword { get; set; }

    [Required(ErrorMessage = "La nueva contraseña es requerida")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva Contraseña")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
        ErrorMessage = "La contraseña debe tener al menos 8 caracteres, una mayúscula, una minúscula, un número y un carácter especial")]
    public string NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirmar Nueva Contraseña")]
    [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmNewPassword { get; set; }
}