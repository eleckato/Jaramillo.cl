using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace jaramillo.cl.Models.ViewModels
{
    public class MechMailVM
    {
        [Required]
        public string user { get; set; }
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Mail Inválido")]
        public string email { get; set; }
        [Required]
        public string message { get; set; }
        public MechMailVM()
        {

        }
    }
}