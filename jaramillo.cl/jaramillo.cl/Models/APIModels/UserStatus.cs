using System;
using System.ComponentModel.DataAnnotations;

namespace jaramillo.cl.Models.APIModels
{
    public class UserStatus
    {
        [Key]
        public string status_id { get; set; }
        public string status { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime created_at { get; set; }
        public bool deleted { get; set; }

        public UserStatus()
        {

        }
    }
}