using System;
using System.ComponentModel.DataAnnotations;

namespace jaramillo.cl.Models.APIModels
{
    public class PublicacionMec
    {
        [Required]
        [Display(Name = "Id")]
        public string public_id { get; set; }

        [Required]
        [Display(Name = "Mecánico")]
        public string appuser_id { get; set; }


        public string user_type_id { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string public_status_id { get; set; }

        [Required]
        [Display(Name = "Fecha de Publicación")]
        public DateTime created_at { get; set; }

        [Required]
        [Display(Name = "Ultima Actualización")]
        public DateTime updated_at { get; set; }

        [Required]
        [Display(Name = "Eliminado")]
        public bool deleted { get; set; }


        [Required]
        [Display(Name = "Titulo")]
        [StringLength(50, ErrorMessage = "Debe tener menos de {1} caracteres")]
        public string title { get; set; }

        [Required]
        [Display(Name = "Descripción")]
        [StringLength(250, ErrorMessage = "Debe tener menos de {1} caracteres")]
        public string public_desc { get; set; }

        [Required]
        [Display(Name = "Horario")]
        [StringLength(30, ErrorMessage = "Debe tener menos de {1} caracteres")]
        public string schedule { get; set; }

        [Required]
        [Display(Name = "Servicios")]
        [StringLength(250, ErrorMessage = "Debe tener menos de {1} caracteres")]
        public string services { get; set; }

        [Display(Name = "Empresa")]
        [StringLength(25, ErrorMessage = "Debe tener menos de {1} caracteres")]
        public string bussiness_name { get; set; }

        [Required]
        [Display(Name = "Dirección")]
        [StringLength(50, ErrorMessage = "Debe tener menos de {1} caracteres")]
        public string address { get; set; }

        [Required]
        [Display(Name = "Comuna")]
        [StringLength(60, ErrorMessage = "Debe tener menos de {1} caracteres")]
        public string comuna { get; set; }

        [Required]
        [Display(Name = "Región")]
        [StringLength(60, ErrorMessage = "Debe tener menos de {1} caracteres")]
        public string region { get; set; }

        [Display(Name = "Teléfono Fijo")]
        [StringLength(20, ErrorMessage = "Debe tener menos de {1} caracteres")]
        public string landline { get; set; }

        [Display(Name = "Celular")]
        [StringLength(20, ErrorMessage = "Debe tener menos de {1} caracteres")]
        public string mobile_number { get; set; }

        [Required]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "El email ingresado no es valido")]
        [StringLength(50, ErrorMessage = "Debe tener menos de {1} caracteres")]
        public string email { get; set; }

        [Display(Name = "Visitas")]
        public int views { get; set; }


        public Usuario user { get; set; }
        public PublicStatus status { get; set; }


        public string mech_name {
            get
            {
                return user?.fullName ?? "ERROR";
            }
        }
        public string status_name { 
            get
            {
                return status?.status_name ?? "ERROR";
            }
        }


        public PublicacionMec()
        {

        }

        public PublicacionMec(bool isTemplate)
        {
            // PK
            public_id = Guid.NewGuid().ToString();
            // FK
            appuser_id = string.Empty;
            user_type_id = string.Empty;
            public_status_id = "PEN";

            // Info
            title = string.Empty;
            public_desc = string.Empty;
            services = string.Empty;
            schedule = string.Empty;
            bussiness_name = string.Empty;
            // Address
            address = string.Empty;
            comuna = string.Empty;
            region = string.Empty;
            // Contact Info
            landline = string.Empty;
            mobile_number = string.Empty;
            email = string.Empty;
            // system
            created_at = DateTime.Today;
            updated_at = DateTime.Today;
            deleted = false;
            // Maybe
            views = 0;
        }
    }
}