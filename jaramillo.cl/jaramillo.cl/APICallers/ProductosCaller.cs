using jaramillo.cl.Common;
using jaramillo.cl.Models;
using jaramillo.cl.Models.APIModels;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace jaramillo.cl.APICallers
{
    public class ProductosCaller : CallerBase
    {
        private readonly string prefix = "supply-adm";
        private readonly string fullPrefix = "supply-adm/product";

        /* ---------------------------------------------------------------- */
        /* PRODUCTOS CALLER */
        /* ---------------------------------------------------------------- */

        // TODO Pagination
        /// <summary>
        /// API call to list all Products
        /// </summary>
        public IEnumerable<Producto> GetAllProd(string brand, string name, string product_status, bool deleted = false)
        {
            try
            {
                var delString = deleted ? "&deleted=true" : "";
                var url = $"{fullPrefix}?brand={brand}&name={name}&product_status={product_status}{delString}";

                // Request Base
                var request = new RestRequest(url, Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                // Ejecutar request y guardar la respuesta
                var response = client.Execute<List<Producto>>(request);

                // Levanta una excepción si el status code es diferente de 200
                CheckStatusCode(response);

                var prods = response.Data;

                var prodStatusLst = GetAllStatus().ToList();
                if (prodStatusLst == null) return null;

                var prodUnitLst = GetAllUnits().ToList();
                if (prodUnitLst == null) return null;

                prods.ForEach(pub =>
                {
                    pub = ProcessProd(pub, prodStatusLst, prodUnitLst);
                });

                // Retorna el producto
                return prods;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }

        /// <summary>
        /// API call to get a Product
        /// </summary>
        /// <param name="prodId"> Product Id </param>
        public Producto GetProd(string prodId)
        {
            if (string.IsNullOrEmpty(prodId))
            {
                ErrorWriter.InvalidArgumentsError();
                return null;
            }

            try
            {
                var request = new RestRequest($"{fullPrefix}/{prodId}", Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<Producto>(request);

                string notFoundMsg = "El Producto requerido no existe";
                CheckStatusCode(response, notFoundMsg);

                var prod = response.Data;

                var prodStatusLst = GetAllStatus().ToList();
                if (prodStatusLst == null) return null;

                var prodUnitLst = GetAllUnits().ToList();
                if (prodUnitLst == null) return null;

                prod = ProcessProd(prod, prodStatusLst, prodUnitLst);

                return prod;
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
        /// API call to get all Product Status, with name and ID
        /// </summary>
        public IEnumerable<ProdStatus> GetAllStatus()
        {
            try
            {
                var request = new RestRequest($"{prefix}/product-status", Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<List<ProdStatus>>(request);

                CheckStatusCode(response);

                return response.Data;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }

        /// <summary>
        /// API call to get all the Product Units
        /// </summary>
        public IEnumerable<ProdUnit> GetAllUnits()
        {
            try
            {
                var request = new RestRequest($"{prefix}/units", Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<List<ProdUnit>>(request);

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
        /// <param name="prod"> Product to process </param>
        /// <param name="prodStatusList"> List with all Product Status </param>
        /// <param name="prodUnitList"> List with all Product Units </param>
        public Producto ProcessProd(Producto prod, List<ProdStatus> prodStatusList, List<ProdUnit> prodUnitList)
        {
            if (prod == null || prodStatusList == null) return null;

            // Set Status
            var thisProdStatus = prodStatusList.FirstOrDefault(status => status.status_id.Equals(prod.product_status));
            prod.status_name = thisProdStatus?.status ?? string.Empty;
            // Set Unit data
            var thisProdUnit = prodUnitList.FirstOrDefault(unit => unit.abbreviation.Equals(prod.unit_id));
            prod.Unit = thisProdUnit;

            return prod;
        }

    }
}