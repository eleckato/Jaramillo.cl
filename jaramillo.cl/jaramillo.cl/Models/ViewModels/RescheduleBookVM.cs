using jaramillo.cl.Models.APIModels;
using System.Collections.Generic;

namespace jaramillo.cl.Models.ViewModels
{
    public class RescheduleBookVM
    {
        public BookingVM booking { get; set; }

        public List<BookingVM> otherBookList { get; set; }
        public List<BookingRestVM> restList { get; set; }
    }
}