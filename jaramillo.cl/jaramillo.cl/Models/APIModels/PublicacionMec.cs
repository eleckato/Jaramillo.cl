using System;
using System.ComponentModel.DataAnnotations;

namespace jaramillo.cl.Models.APIModels
{
    public class PublicacionMec
    {
        [Display(Name = "Id")]
        public string public_id { get; set; }

        [Display(Name = "Mecánico")]
        public string appuser_id { get; set; }
        public string mech_name { get; set; }


        public string user_type_id { get; set; }

        [Display(Name = "Status")]
        public string public_status_id { get; set; }

        public string status_name { get; set; }

        [Display(Name = "Fecha de Publicación")]
        public DateTime created_at { get; set; }

        [Display(Name = "Ultima Actualización")]
        public DateTime updated_at { get; set; }

        [Display(Name = "Eliminado")]
        public bool deleted { get; set; }

        [Display(Name = "Titulo")]
        public string title { get; set; }

        [Display(Name = "Descripción")]
        public string public_desc { get; set; }

        [Display(Name = "Horario")]
        public string schedule { get; set; }

        [Display(Name = "Servicios")]
        public string services { get; set; }

        [Display(Name = "Empresa")]
        public string bussiness_name { get; set; }

        [Display(Name = "Dirección")]
        public string address { get; set; }

        [Display(Name = "Comuna")]
        public string comuna { get; set; }

        [Display(Name = "Región")]
        public string region { get; set; }

        [Display(Name = "Teléfono Fijo")]
        public string landline { get; set; }

        [Display(Name = "Celular")]
        public string mobile_number { get; set; }

        [Display(Name = "Email")]
        public string email { get; set; }

        [Display(Name = "Visitas")]
        public int views { get; set; }

        public PublicacionMec()
        {

        }

        public PublicacionMec(bool isTemplate)
        {
            public_id = Guid.NewGuid().ToString();
            appuser_id = string.Empty;
            user_type_id = string.Empty;
            public_status_id = string.Empty;
            created_at = DateTime.Today;
            updated_at = DateTime.Today;
            deleted = false;
            title = string.Empty;
            public_desc = string.Empty;
            services = string.Empty;
            schedule = string.Empty;
            bussiness_name = string.Empty;
            address = string.Empty;
            comuna = string.Empty;
            region = string.Empty;
            landline = string.Empty;
            mobile_number = string.Empty;
            email = string.Empty;
            views = 0;
        }
    }
}