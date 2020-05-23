using System;
using System.ComponentModel.DataAnnotations;

namespace jaramillo.cl.Models.APIModels
{
    public class UserType
    {
        [Key]
        public string user_type_id { get; set; }
        public string name { get; set; }

        public DateTime updated_at { get; set; }
        public DateTime created_at { get; set; }
        public bool deleted { get; set; }

        public UserType()
        {

        }
    }
}