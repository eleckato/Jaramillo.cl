using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace jaramillo.cl.Models.APIModels
{
    public class PublicStatus
    {
        public string public_status_id { get; set; }
        public string status_name { get; set; }
        public DateTime created_at { get; set; }
        public DateTime update_at { get; set; }
        public bool deleted { get; set; }
        public PublicStatus()
        {

        }
    }
}