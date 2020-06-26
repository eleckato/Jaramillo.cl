using System;
using System.ComponentModel.DataAnnotations;


namespace jaramillo.cl.Models.APIModels
{
    public class Usuario
    {
        public string appuser_id { get; set; }

        [Required]
        [Display(Name = "Username")]
        [StringLength(15, ErrorMessage = "Debe tener menos de {1} caracteres")]
        public string username { get; set; }

        [Required]
        [Display(Name = "Contraseña")]
        [StringLength(30, ErrorMessage = "Debe tener menos de {1} caracteres")]
        public string hash { get; set; }

        [Required]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "El email ingresado no es valido")]
        [StringLength(30, ErrorMessage = "Debe tener menos de {1} caracteres")]
        public string email { get; set; }

        [Required]
        [Display(Name = "Nombres")]
        [StringLength(25, ErrorMessage = "Debe tener menos de {1} caracteres")]
        public string name { get; set; }

        [Required]
        [Display(Name = "Apellidos")]
        [StringLength(25, ErrorMessage = "Debe tener menos de {1} caracteres")]
        public string last_names { get; set; }

        [Required]
        [Display(Name = "RUT")]
        [StringLength(12, ErrorMessage = "Debe tener menos de {1} caracteres")]
        public string rut { get; set; }

        [Required]
        [Display(Name = "Dirección")]
        [StringLength(50, ErrorMessage = "Debe tener menos de {1} caracteres")]
        public string adress { get; set; }

        [Required]
        [Display(Name = "Teléfono")]
        [StringLength(15, ErrorMessage = "Debe tener menos de {1} caracteres")]
        public string phone { get; set; }


        [Required]
        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public DateTime birthday { get; set; }


        [Required]
        [Display(Name = "Último Login")]
        [DataType(DataType.Date)]
        public DateTime lastlogin { get; set; }


        [Required]
        [Display(Name = "Email Confirmado")]
        public bool mail_confirmed { get; set; }

        [Required]
        [Display(Name = "Tipo")]
        public string user_type_id { get; set; }

        public string user_type_name { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string status_id { get; set; }

        public string status_name { get; set; }

        [Required]
        [Display(Name = "Ultima Actualización")]
        [DataType(DataType.Date)]
        public DateTime updated_at { get; set; }

        [Required]
        [Display(Name = "Fecha de Registro")]
        [DataType(DataType.Date)]
        public DateTime created_at { get; set; }

        public bool deleted { get; set; }


        [Display(Name = "Nombre")]
        public string fullName
        {
            get { return $"{name} {last_names}"; }
        }


        public Usuario()
        {

        }

        public Usuario(bool isTemplate)
        {
            appuser_id = string.Empty;
            username = string.Empty;
            email = string.Empty;
            name = string.Empty;
            last_names = string.Empty;
            rut = string.Empty;
            adress = string.Empty;
            phone = string.Empty;
            lastlogin = DateTime.Today;
            mail_confirmed = false;
            user_type_id = string.Empty;
            user_type_name = string.Empty;
            status_id = string.Empty;
            status_name = string.Empty;
            updated_at = DateTime.Today;
            created_at = DateTime.Today;
            deleted = false;
        }
    }
}