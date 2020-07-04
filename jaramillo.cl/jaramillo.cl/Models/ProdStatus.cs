using System;

namespace jaramillo.cl.Models
{
    public class ProdStatus
    {
        public string status_id { get; set; }
        public string status { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool deleted { get; set; }

        public ProdStatus()
        {

        }
    }
}