using System;

namespace jaramillo.cl.Models.APIModels
{
    public class BookingStatus
    {
        public string status_booking_id { get; set; }
        public string name { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime created_at { get; set; }
        public bool deleted { get; set; }

        public BookingStatus()
        {

        }
    }
}