using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;

namespace jaramillo.cl.Models.APIModels
{
    public class Producto
    {
        public string product_id { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        [StringLength(30, ErrorMessage = "Debe tener menos de {1} caracteres")]
        public string name { get; set; }

        [Required]
        [Display(Name = "Descripción")]
        [StringLength(250, ErrorMessage = "Debe tener menos de {1} caracteres")]
        public string product_desc { get; set; }


        [Required]
        [Display(Name = "Precio")]
        public int price { get; set; }

        [Required]
        [Display(Name = "Stock")]
        [Range(1, 9999999999, ErrorMessage = "Debe tener menos de 10 dígitos y ser mayor a 0")]
        public int stock { get; set; }

        [Required]
        [Display(Name = "Alerta de Stock")]
        [Range(1, 9999999999, ErrorMessage = "Debe tener menos de 10 dígitos y ser mayor a 0")]
        public int stock_alert { get; set; }

        [Required]
        [Display(Name = "Marca")]
        [StringLength(30, ErrorMessage = "Debe tener menos de {1} caracteres")]
        public string brand { get; set; }

        [Required]
        [Display(Name = "Fecha de Creación")]
        public DateTime created_at { get; set; }

        [Required]
        [Display(Name = "Ultima Actualización")]
        public DateTime updated_at { get; set; }

        [Required]
        [Display(Name = "Eliminado")]
        public bool deleted { get; set; }

        [Required]
        [Display(Name = "Unidad")]
        public string unit_id { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string product_status { get; set; }


        [Required]
        [Display(Name = "Precio")]
        public string priceString
        {
            get
            {
                return price.ToString("C", CultureInfo.CreateSpecificCulture("es-CL"));
            }
        }

        public string stockString
        {
            get
            {
                string unitName = Unit?.name ?? "";
                string unitPruralName = Unit?.plural_name ?? "";

                string str = $"{stock} {(stock == 1 ? unitName : unitPruralName)}";
                return str;
            }
        }

        public string stockAlertString
        {
            get
            {
                string unitName = Unit?.name ?? "";
                string unitPruralName = Unit?.plural_name ?? "";

                string str = $"{stock_alert} {(stock_alert == 1 ? unitName : unitPruralName)}";
                return str;
            }
        }

        public ProdUnit Unit { get; set; }

        public string status_name { get; set; }

        public Producto()
        {

        }

        public Producto(bool isTemplate)
        {
            product_id = string.Empty;
            name = string.Empty;
            product_desc = string.Empty;

            price = 0;
            stock = 0;
            stock_alert = 0;
            brand = string.Empty;

            product_status = string.Empty;
            status_name = string.Empty;

            unit_id = string.Empty;
            Unit = new ProdUnit();

            created_at = DateTime.Now;
            updated_at = DateTime.Now;
            deleted = false;
        }
    }
}