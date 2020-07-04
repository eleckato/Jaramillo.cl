using System;

namespace jaramillo.cl.Models.APIModels
{
    public class ProdUnit
    {
        public string abbreviation { get; set; }
        public string name { get; set; }
        public string plural_name { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool deleted { get; set; }

        public ProdUnit()
        {

        }
    }
}