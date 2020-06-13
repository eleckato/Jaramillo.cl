using jaramillo.cl.Models.APIModels;
using jaramillo.cl.Common;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace jaramillo.cl.APICallers
{
    public class ServCaller : CallerBase
    {
        private readonly string prefix = "supply-adm";
        private readonly string fullPrefix = "supply-adm/service";

        /* ---------------------------------------------------------------- */
        /* SERVICES CALLER */
        /* ---------------------------------------------------------------- */

        // TODO Pagination
        /// <summary>
        /// API call to list all Services
        /// </summary>
        public IEnumerable<Servicio> GetAllServ(string name, string serv_status, bool deleted = false)
        {
            try
            {
                var delString = deleted ? "&deleted=true" : "";
                var url = $"{fullPrefix}?name={name}&serv_status={serv_status}{delString}";

                // Request Base
                var request = new RestRequest(url, Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                // Ejecutar request y guardar la respuesta
                var response = client.Execute<List<Servicio>>(request);

                // Levanta una excepción si el status code es diferente de 200
                CheckStatusCode(response);

                var servs = response.Data;

                // Retorna el producto
                return servs;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }

        /// <summary>
        /// API call to get a Service
        /// </summary>
        /// <param name="servId"> Service Id </param>
        public Servicio GetServ(string servId)
        {
            if (string.IsNullOrEmpty(servId))
            {
                ErrorWriter.InvalidArgumentsError();
                return null;
            }

            try
            {
                var request = new RestRequest($"{fullPrefix}/{servId}", Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<Servicio>(request);

                string notFoundMsg = "El Servicio requerido no existe";
                CheckStatusCode(response, notFoundMsg);


                var serv = response.Data;

                return serv;
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
        /// API call to get all Service Status, with name and ID
        /// </summary>
        public IEnumerable<ServStatus> GetAllStatus()
        {
            try
            {
                var request = new RestRequest($"{prefix}/serv_status", Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<List<ServStatus>>(request);

                CheckStatusCode(response);

                return response.Data;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }

    }
}