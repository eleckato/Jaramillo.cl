using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace jaramillo.cl.Models.APIModels
{
    public class Sale
    {
        [Required]
        [Display(Name = "Id")]
        public string sale_id { get; set; }

        [Required]
        public string seller_id { get; set; }
        public string cashier_id { get; set; }
        public string appuser_id { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string sale_status_id { get; set; }

        [Required]
        [Display(Name = "Fecha de la Orden")]
        public DateTime? date_order { get; set; }

        [Display(Name = "Fecha de la Orden")]
        public string dateOrderString
        {
            get
            {
                return date_order?.ToString("dd/MM/yyyy HH:mm") ?? "No date";
            }
        }

        [Required]
        [Display(Name = "Fecha de la Venta")]
        public DateTime? sale_date { get; set; }

        [Display(Name = "Fecha de la Venta")]
        public string saleDateString
        {
            get
            {
                return sale_date?.ToString("dd/MM/yyyy HH:mm") ?? "-";
            }
        }


        [Required]
        [Display(Name = "Código")]
        public long code { get; set; }
        [Required]
        [Display(Name = "Método de Pago")]
        public string payment_method { get; set; }
        [Required]
        [Display(Name = "Subtotal")]
        public int subtotal { get; set; }
        [Required]
        [Display(Name = "Total")]
        public int total { get; set; }

        [Required]
        [Display(Name = "Fecha de Creación")]
        public DateTime created_at { get; set; }
        [Required]
        [Display(Name = "Última actualización")]
        public DateTime updated_at { get; set; }
        [Required]
        [Display(Name = "Eliminado")]
        public bool deleted { get; set; }

        public Sale()
        {

        }

    }

    public class SaleVM : Sale
    {
        [Required]
        [Display(Name = "Vendedor")]
        public Usuario seller { get; set; }
        [Required]
        [Display(Name = "Cajero")]
        public Usuario cashier { get; set; }
        [Required]
        [Display(Name = "Usuario")]
        public Usuario user { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string sale_status_name { get; set; }

        public List<SaleItemVM> saleItems { get; set; }

        public string totalString
        {
            get
            {
                return total.ToString("C", CultureInfo.CreateSpecificCulture("es-CL"));
            }
        }

        public SaleVM()
        {
            seller = null;
            cashier = null;
            user = null;
            sale_status_name = string.Empty;
        }
    }

}