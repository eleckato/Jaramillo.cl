using System;

namespace jaramillo.cl.Models.APIModels
{
    public class SaleStatus
    {
        public string sale_status_id { get; set; }
        public string name { get; set; }

        public DateTime updated_at { get; set; }
        public DateTime created_at { get; set; }
        public bool deleted { get; set; }

        public SaleStatus()
        {

        }

        public SaleStatus(bool isTemplate)
        {
            sale_status_id = string.Empty;
            name = string.Empty;

            updated_at = DateTime.Now;
            created_at = DateTime.Now;
            deleted = false;
        }
    }
}