using jaramillo.cl.Models.APIModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace jaramillo.cl.Models.ViewModels
{
    public class UserServVM
    {
        [Required]
        [Display(Name = "Servicio")]
        public Servicio serv { get; set; }

        [Required]
        [Display(Name = "Fecha")]
        public DateTime date { get; set; }

        [Required]
        [Display(Name = "Total")]
        public int total { get; set; }



        [Display(Name = "Fecha")]
        public string dateString
        {
            get
            {
                return date.ToString("dd/MM/yyyy HH:mm") ?? "No date";
            }
        }

        [Required]
        [Display(Name = "Precio")]
        public string totalString
        {
            get
            {
                return total.ToString("C", CultureInfo.CreateSpecificCulture("es-CL"));
            }
        }


        public UserServVM()
        {

        }

    }
}