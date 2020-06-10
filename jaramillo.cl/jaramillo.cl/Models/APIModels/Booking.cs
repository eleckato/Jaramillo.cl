using System;
using System.ComponentModel.DataAnnotations;

namespace jaramillo.cl.Models.APIModels
{
    public class Booking
    {
        [Required]
        [Display(Name = "Id")]
        public string booking_id { get; set; }

        [Required]
        [Display(Name = "Servicio")]
        public string serv_id { get; set; }

        [Required]
        [Display(Name = "Usuario")]
        public string appuser_id { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string status_booking_id { get; set; }

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

        public Booking()
        {

        }

        public Booking(bool isTemplate)
        {
            booking_id = Guid.NewGuid().ToString();

            appuser_id = null;

            status_booking_id = "ACT";
            updated_at = null;
            created_at = null;
            deleted = false;

            serv_id = null;
            start_date_hour = null;
            end_date_hour = null;
        }
    }

    public class BookingVM : Booking
    {
        [Display(Name = "Servicio")]
        public Servicio serv { get; set; }

        [Display(Name = "Usuario")]
        public Usuario user { get; set; }



        [Display(Name = "Usuario")]
        public string userName { get { return user?.fullName ?? Resources.Messages.StringNotFound; } }

        [Display(Name = "Servicio")]
        public string servName { get { return serv?.name ?? Resources.Messages.StringNotFound; } }


        [Display(Name = "Comienzo")]
        public string startDateTimeString { get { return start_date_hour?.ToString("dd/MM/yyyy HH:mm") ?? "-"; } }

        [Display(Name = "Término")]
        public string endDateTimeString { get { return end_date_hour?.ToString("dd/MM/yyyy HH:mm") ?? "-"; } }

        [Display(Name = "Fecha")]
        public string date { get { return start_date_hour?.ToString("dd/MM/yyyy") ?? string.Empty; } }

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
        public string updatedAtString { get { return updated_at?.ToString("dd/MM/yyyy HH:mm") ?? "-"; } }

        [Display(Name = "Fecha de Registro")]
        public string createdAtString { get { return created_at?.ToString("dd/MM/yyyy HH:mm") ?? "-"; } }

        [Display(Name = "Status")]
        public string statusName { get; set; }
    }
}