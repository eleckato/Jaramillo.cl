using System;
using System.ComponentModel.DataAnnotations;


namespace jaramillo.cl.Models.APIModels
{
    public class StoreSchedule
    {
        [Required]
        [Display(Name = "Id")]
        public string schedule_id { get; set; }

        [Required]
        [Display(Name = "Apertura")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]

        public DateTime start_hour { get; set; }

        [Required]
        [Display(Name = "Comienzo Hora de Almuerzo")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]

        public DateTime start_lunch_hour { get; set; }

        [Required]
        [Display(Name = "Termino Hora de Almuerzo")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]

        public DateTime end_lunch_hour { get; set; }

        [Required]
        [Display(Name = "Cierre")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]

        public DateTime end_hour { get; set; }

        public DateTime updated_at { get; set; }
        public DateTime created_at { get; set; }
        public bool deleted { get; set; }
        public StoreSchedule()
        {

        }
    }
}