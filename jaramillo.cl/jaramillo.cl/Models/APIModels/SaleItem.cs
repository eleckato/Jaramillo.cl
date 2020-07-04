using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace jaramillo.cl.Models.APIModels
{
    public class SaleItem
    {
        [Required]
        [Display(Name = "Id")]
        public string provision_id { get; set; }

        [Required]
        [Display(Name = "Venta")]
        public string sale_id { get; set; }

        [Display(Name = "Producto")]
        public string product_id { get; set; }

        [Display(Name = "Servicio")]
        public string serv_id { get; set; }

        [Required]
        [Display(Name = "Cantidad")]
        public int quantity { get; set; }

        [Required]
        [Display(Name = "Precio")]
        public int unit_price { get; set; }

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

        public SaleItem()
        {

        }

        public SaleItem(bool isTemplate)
        {
            provision_id = string.Empty;

            sale_id = string.Empty;
            product_id = string.Empty;
            serv_id = string.Empty;

            quantity = 0;
            unit_price = 0;
            total = 0;

            updated_at = updated_at;
            created_at = created_at;
            deleted = deleted;
        }
    }

    public class SaleItemVM : SaleItem
    {

        [Display(Name = "Producto")]
        public Producto prod { get; set; }

        [Display(Name = "Servicio")]
        public Servicio serv { get; set; }


        [Display(Name = "Total")]
        public string totalString
        {
            get
            {
                return total.ToString("C", CultureInfo.CreateSpecificCulture("es-CL"));
            }
        }


        [Display(Name = "Precio")]
        public string unit_priceString
        {
            get
            {
                return unit_price.ToString("C", CultureInfo.CreateSpecificCulture("es-CL"));
            }
        }


        [Display(Name = "Cantidad")]
        public string quantityString
        {
            get
            {
                if (!checkIntegrity()) return "Data inconsistente";
                if (prod != null)
                {
                    string unitName = prod.Unit?.name ?? "";
                    string unitPruralName = prod.Unit?.plural_name ?? "";

                    string str = $"{quantity} {(quantity == 1 ? unitName : unitPruralName)}";

                    return str;
                }

                return "-";
            }
        }


        [Display(Name = "Tipo")]
        public string type
        {
            get
            {
                if (!checkIntegrity()) return "Data inconsistente";
                return (prod != null) ? "Producto" : "Servicio";
            }
        }

        public string itemName
        {
            get
            {
                var res = prod?.name ?? serv?.name ?? "No Data";
                return res;
            }
        }

        public SaleItemVM()
        {

        }

        private bool checkIntegrity()
        {
            if (prod == null && serv == null) return false;
            if (prod != null && serv != null) return false;
            return true;
        }
    }
}