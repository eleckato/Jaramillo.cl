using System;
using System.ComponentModel.DataAnnotations;

namespace jaramillo.cl.Models.APIModels
{
    public class BookingRestriction
    {
        [Required]
        [Display(Name = "Id")]
        public string restriction_id { get; set; }

        [Required]
        [Display(Name = "Servicio")]
        public string serv_id { get; set; }

        [Required]
        [Display(Name = "Última Actualización")]
        public DateTime? updated_at { get; set; }

        [Required]
        [Display(Name = "Fecha de Registro")]
        public DateTime? created_at { get; set; }

        public bool deleted { get; set; }

        [Required]
        [Display(Name = "Comienzo")]
        [DataType(DataType.DateTime)]
        public DateTime? start_date_hour { get; set; }
        [Required]
        [Display(Name = "Término")]
        [DataType(DataType.DateTime)]
        public DateTime? end_date_hour { get; set; }

        public BookingRestriction()
        {

        }
    }

    public class BookingRestVM : BookingRestriction
    {
        private readonly string dateFormat = "dd/MM/yyyy HH:mm";

        [Display(Name = "Servicio")]
        public Servicio serv { get; set; }


        [Display(Name = "Servicio")]
        public string servName { get { return serv?.name ?? Resources.Messages.StringNotFound; } }

        [Display(Name = "Comienzo")]
        public string startDateTimeString { get { return start_date_hour?.ToString(dateFormat) ?? "-"; } }

        [Display(Name = "Término")]
        public string endDateTimeString { get { return end_date_hour?.ToString(dateFormat) ?? "-"; } }


        [Display(Name = "Horario")]
        public string schedule
        {
            get
            {
                string date = start_date_hour?.ToString("dd/MM/yyyy") ?? string.Empty;
                string startTime = start_date_hour?.ToString("HH:mm") ?? string.Empty;
                string endTime = end_date_hour?.ToString("HH:mm") ?? string.Empty;

                if (string.IsNullOrEmpty(date) || string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
                    return "ERROR";

                string final = $"{date} | {startTime} - {endTime}";
                return final;
            }
        }


        [Display(Name = "Última Actualización")]
        public string updatedAtString { get { return updated_at?.ToString(dateFormat) ?? "-"; } }

        [Display(Name = "Fecha de Registro")]
        public string createdAtString { get { return created_at?.ToString(dateFormat) ?? "-"; } }

        public BookingRestVM()
        {

        }
    }
}