using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace jaramillo.cl.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Nombre de Usuario")]
        public string LoginUsername { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string LoginPassword { get; set; }

        [Display(Name = "Mantener Sesión Iniciada")]
        public bool RememberMe { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "El campo es obligatorio")]
        [EmailAddress(ErrorMessage = "No es un mail válido")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
