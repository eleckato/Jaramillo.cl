using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace jaramillo.cl.Models.APIModels
{
    public class ServStatus
    {
        public string status_id { get; set; }
        public string status { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool deleted { get; set; }

        public ServStatus()
        {

        }

        public ServStatus(bool isTemplate)
        {
            status_id = string.Empty;
            status = string.Empty;
            created_at = DateTime.Now;
            updated_at = DateTime.Now;
            deleted = false;
        }
    }

}