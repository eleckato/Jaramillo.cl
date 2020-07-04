using jaramillo.cl.Common;
using jaramillo.cl.Models.APIModels;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace jaramillo.cl.APICallers
{
    public class SalesCaller : CallerBase
    {
        private readonly string prefix = "sale";
        private readonly string salesPrefix = "sale/sales";


        // TODO Pagination
        /// <summary>
        /// API call to list all Sales
        /// </summary>
        public IEnumerable<SaleVM> GetAllSales(string code, string sale_status_id, bool deleted = false, string id_cashier = "", string id_seller = "", string id_appuser = "")
        {
            try
            {
                var delString = deleted ? "&deleted=true" : "";
                var url = $"{salesPrefix}?code={code}&id_cashier={id_cashier}&id_seller={id_seller}&id_appuser={id_appuser}&sale_status_id={sale_status_id}{delString}";

                // Request Base
                var request = new RestRequest(url, Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                // Ejecutar request y guardar la respuesta
                var response = client.Execute<List<SaleVM>>(request);

                // Levanta una excepción si el status code es diferente de 200
                CheckStatusCode(response);

                var sales = response.Data;

                // Data para conseguir la información más profunda de la venta
                var saleStatusList = GetAllStatus().ToList();
                if (saleStatusList == null) return null;

                var userList = new UsuariosCaller().GetAllUsers(string.Empty, string.Empty, string.Empty, "ACT").ToList();

                sales.ForEach(sale =>
                {
                    sale = ProcessSale(sale, saleStatusList, userList);
                });

                // Retorna las ventas
                return sales;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }


        /// <summary>
        /// API call to get a Sale
        /// </summary>
        /// <param name="saleId"> Sale Id </param>
        public SaleVM GetSale(string saleId)
        {
            if (string.IsNullOrEmpty(saleId))
            {
                ErrorWriter.InvalidArgumentsError();
                return null;
            }

            try
            {
                var request = new RestRequest($"{salesPrefix}/{saleId}", Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<SaleVM>(request);

                string notFoundMsg = "La venta requerida no existe";
                CheckStatusCode(response, notFoundMsg);


                var sale = response.Data;

                var saleStatusList = GetAllStatus().ToList();
                if (saleStatusList == null) return null;

                var userList = new UsuariosCaller().GetAllUsers(string.Empty, string.Empty, string.Empty, "ACT").ToList();

                sale = ProcessSale(sale, saleStatusList, userList);

                // Agregar los Items de la venta ya que es el detalle
                var saleItems = GetSaleItems(sale.sale_id);
                sale.saleItems = saleItems.ToList();

                return sale;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }


        /* ---------------------------------------------------------------- */
        /* SALE ITEM */
        /* ---------------------------------------------------------------- */

        // TODO Pagination
        /// <summary>
        /// API call to list all Sale Items of a Sale
        /// </summary>
        public IEnumerable<SaleItemVM> GetSaleItems(string saleId)
        {
            if (string.IsNullOrEmpty(saleId))
            {
                ErrorWriter.InvalidArgumentsError();
                return null;
            }

            try
            {
                var request = new RestRequest($"{prefix}/provisions/{saleId}", Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<List<SaleItemVM>>(request);

                string notFoundMsg = "Items de la venta no encontrados";
                CheckStatusCode(response, notFoundMsg);

                var saleItems = response.Data;


                var prodList = new ProductosCaller().GetAllProd(string.Empty, string.Empty, string.Empty).ToList();
                var delProdList = new ProductosCaller().GetAllProd(string.Empty, string.Empty, string.Empty, true).ToList();

                prodList.AddRange(delProdList);

                var servList = new ServCaller().GetAllServ(string.Empty, string.Empty).ToList();
                var delServList = new ServCaller().GetAllServ(string.Empty, string.Empty, true).ToList();

                servList.AddRange(delServList);

                saleItems.ForEach(x =>
                {
                    x = ProcessSaleItem(x, prodList, servList);
                });

                return saleItems;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }


        /* ---------------------------------------------------------------- */
        /* GET SECONDARY DATA */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// API call to get all Sale Status, with name and ID
        /// </summary>
        public IEnumerable<SaleStatus> GetAllStatus()
        {
            try
            {
                var request = new RestRequest($"{prefix}/sale_status", Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<List<SaleStatus>>(request);

                CheckStatusCode(response);

                return response.Data;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }


        /* ---------------------------------------------------------------- */
        /* HELPERS */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// Set all the secondary data ,like getting the status Name from the status Id
        /// </summary>
        /// <param name="sale"> Sale to process </param>
        /// <param name="saleStatusList"> List with all Sale Status </param>
        /// <param name="userList"> List of Users </param>
        public SaleVM ProcessSale(SaleVM sale, List<SaleStatus> saleStatusList, List<Usuario> userList)
        {
            if (sale == null || saleStatusList == null || userList == null) return null;

            // Set Status
            var thisSaleStatus = saleStatusList.FirstOrDefault(status => status.sale_status_id.Equals(sale.sale_status_id));
            sale.sale_status_name = thisSaleStatus?.name ?? string.Empty;

            // Set Seller
            var thisSeller = userList.FirstOrDefault(x => x.appuser_id.Equals(sale.seller_id));
            sale.seller = thisSeller ?? null;

            // Set Cashier
            var thisCashier = userList.FirstOrDefault(x => x.appuser_id.Equals(sale.cashier_id));
            sale.cashier = thisCashier ?? null;

            // Set User
            var thisUser = userList.FirstOrDefault(x => x.appuser_id.Equals(sale.appuser_id));
            sale.user = thisUser ?? null;

            return sale;
        }

        /// <summary>
        /// Set all the secondary data ,like getting the status Name from the status Id
        /// </summary>
        /// <param name="saleItem"> Sale Item to process </param>
        public SaleItemVM ProcessSaleItem(SaleItemVM saleItem, List<Producto> prodList, List<Servicio> servList)
        {
            if (saleItem == null || prodList == null || servList == null) return null;

            // Set Product
            if (!string.IsNullOrEmpty(saleItem.product_id))
            {
                var thisProd = prodList.FirstOrDefault(x => x.product_id.Equals(saleItem.product_id));
                saleItem.prod = thisProd ?? null;
            }

            // Set Service
            if (!string.IsNullOrEmpty(saleItem.serv_id))
            {
                var thisServ = servList.FirstOrDefault(x => x.serv_id.Equals(saleItem.serv_id));
                saleItem.serv = thisServ ?? null;
            }

            return saleItem;
        }

    }
}