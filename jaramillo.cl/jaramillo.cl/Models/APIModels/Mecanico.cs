using System;
using System.ComponentModel.DataAnnotations;


namespace jaramillo.cl.Models.APIModels
{
    public class Mecanico : Usuario
    {

        [Display(Name = "Publicación Pendiente")]
        public bool hasPendingPublication { get; set; }

        [Display(Name = "Tiene Deuda")]
        public bool hasDebt { get; set; }

        public Mecanico()
        {

        }

        public Mecanico(bool isTemplate)
        {
            appuser_id = string.Empty;
            username = string.Empty;
            email = string.Empty;
            name = string.Empty;
            last_names = string.Empty;
            adress = string.Empty;
            phone = string.Empty;
            lastlogin = DateTime.Today;
            mail_confirmed = false;
            user_type_id = "MEC";
            user_type_name = string.Empty;
            status_id = string.Empty;
            status_name = string.Empty;
            updated_at = DateTime.Today;
            created_at = DateTime.Today;
            deleted = false;
            hasPendingPublication = false;
            hasDebt = false;
        }

    }
}