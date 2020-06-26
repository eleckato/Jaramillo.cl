using jaramillo.cl.Models.APIModels;
using System.ComponentModel.DataAnnotations;

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
        [StringLength(500, ErrorMessage = "El debe tener menos de 500 caracteres")]
        public string message { get; set; }

        public PublicacionMec publication;

        public MechMailVM()
        {

        }
    }
}