using jaramillo.cl.Common;
using jaramillo.cl.Models.APIModels;
using jaramillo.cl.Providers;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;

namespace jaramillo.cl.APICallers
{
    public class MecanicosCaller : CallerBase
    {
        private readonly string prefix = "mech-adm";

        /* ---------------------------------------------------------------- */
        /* MECHANIC CRUD */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// API call to add an Mechanic
        /// </summary>
        /// <param name="newMech"> New Mechanic model with the data </param>
        public string AddMech(Usuario newMech)
        {
            if (newMech == null)
            {
                ErrorWriter.InvalidArgumentsError();
                return null;
            }

            try
            {
                var userId = newMech.appuser_id;

                var request = new RestRequest("user-auth/register", Method.POST)
                {
                    RequestFormat = DataFormat.Json
                };

                newMech.hash = JwtProvider.EncryptHMAC(newMech.hash);
                request.AddJsonBody(newMech);

                var response = client.Execute(request);

                if (response.StatusCode == HttpStatusCode.Conflict)
                    throw new Exception("El Nombre de Usuario o Mail ya existe");

                CheckStatusCode(response);

                return response.Content;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }

        // TODO Pagination
        /// <summary>
        /// API call to list all Mechanics
        /// </summary>
        public IEnumerable<Mecanico> GetAllMech(string userName, string userRut, string userStatusId, bool deleted = false)
        {
            try
            {
                var delString = deleted ? "&deleted=true" : "";
                var url = $"{prefix}/mechanics?username={userName}&rut={userRut}&status_id={userStatusId}{delString}";

                var request = new RestRequest(url, Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<List<Mecanico>>(request);

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
        /// API call to get a Mechanic
        /// </summary>
        /// <param name="mechId"> Mechanic Id </param>
        public Mecanico GetMech(string mechId)
        {
            if (string.IsNullOrEmpty(mechId))
            {
                ErrorWriter.InvalidArgumentsError();
                return null;
            }

            try
            {
                var request = new RestRequest($"{prefix}/mechanics/{mechId}", Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<Mecanico>(request);

                string notFoundMsg = "El Mecánico requerido no existe";
                CheckStatusCode(response, notFoundMsg);

                return response.Data;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }

        /// <summary>
        /// API call to update an Mechanic
        /// </summary>
        /// <param name="newMech"> New Mechanic model with the data </param>
        public bool UpdateMech(Usuario newMech)
        {
            if (newMech == null)
            {
                ErrorWriter.InvalidArgumentsError();
                return false;
            }

            try
            {
                var userId = newMech.appuser_id;

                var request = new RestRequest($"{prefix}/mechanics/{userId}", Method.POST)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddJsonBody(newMech);

                var response = client.Execute(request);

                string notFoundMsg = "El Mecánico requerido no existe";
                CheckStatusCode(response, notFoundMsg);

                return true;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }


        /// <summary>
        /// API call to change the Status of a Mechanic
        /// </summary>
        /// <param name="mechId"> User Id </param>
        /// <param name="statusId"> User Status Id </param>
        public bool ChangeMechStatus(string mechId, string statusId)
        {
            if (string.IsNullOrEmpty(mechId) || string.IsNullOrEmpty(statusId))
            {
                ErrorWriter.InvalidArgumentsError();
                return false;
            }

            try
            {
                string url = $"{prefix}/mechanics/{mechId}/change-status?status={statusId}";

                var request = new RestRequest(url, Method.POST);

                var response = client.Execute(request);

                // Throw an exception if the StatusCode is different from 200
                CheckStatusCode(response);

                return true;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }
    }
}