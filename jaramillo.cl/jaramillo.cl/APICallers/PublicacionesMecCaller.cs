using jaramillo.cl.Common;
using jaramillo.cl.Models.APIModels;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace jaramillo.cl.APICallers
{
    public class PublicacionesMecCaller : CallerBase
    {
        private readonly string prefix = "pub-adm";

        /* ---------------------------------------------------------------- */
        /* PUBLICATION CRUD */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// API call to list all the Mechanic Publications
        /// </summary>
        public IEnumerable<PublicacionMec> GetAllPub(string comuna, string statusId, string bussName, string title, bool deleted = false)
        {
            try
            {
                var delString = deleted ? "&deleted=true" : "";
                string url = $"{prefix}/publications?comuna={comuna}&public_status_id={statusId}&bussiness_name={bussName}&title={title}{delString}";
                var request = new RestRequest(url, Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<List<PublicacionMec>>(request);

                CheckStatusCode(response);

                var pubList = response.Data;

                foreach (var pub in pubList)
                {
                    if (pub.mobile_number?.Equals("0") ?? false) pub.mobile_number = null;
                    if (pub.landline?.Equals("0") ?? false) pub.landline = null;
                }

                return pubList;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return null;
            }
        }

        /// <summary>
        /// API call to get a Mechanic Publication
        /// </summary>
        /// <param name="pubId"> Publication Id </param>
        public PublicacionMec GetPub(string pubId)
        {
            if (string.IsNullOrEmpty(pubId))
            {
                ErrorWriter.InvalidArgumentsError();
                return null;
            }

            try
            {
                var request = new RestRequest($"{prefix}/publications/{pubId}", Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<PublicacionMec>(request);

                string notFoundMsg = "La Publicación requerida no existe";
                CheckStatusCode(response, notFoundMsg);

                var pub = response.Data;

                if (pub.mobile_number?.Equals("0") ?? false) pub.mobile_number = null;
                if (pub.landline?.Equals("0") ?? false) pub.landline = null;

                return pub;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return null;
            }
        }

        /// <summary>
        /// API call to get add a Mechanic Publication
        /// </summary>
        /// <param name="newPub"> New Publication model with the data </param>
        public string AddPub(PublicacionMec newPub)
        {
            if (newPub == null)
            {
                ErrorWriter.InvalidArgumentsError();
                return null;
            }

            try
            {

                var request = new RestRequest($"{prefix}/publications", Method.POST)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddJsonBody(newPub);

                var response = client.Execute(request);

                string notFoundMsg = "La Publicación requerida no existe";
                CheckStatusCode(response, notFoundMsg);

                return response.Content;

            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return null;
            }
        }

        /// <summary>
        /// API call to update a Mechanic Publication
        /// </summary>
        /// <param name="newPub"> New Publication model with the data </param>
        public bool UpdatePub(PublicacionMec newPub)
        {
            if (newPub == null)
            {
                ErrorWriter.InvalidArgumentsError();
                return false;
            }

            try
            {
                var pubId = newPub.public_id;

                var request = new RestRequest($"{prefix}/publications/{pubId}", Method.POST)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddJsonBody(newPub);

                var response = client.Execute(request);

                string notFoundMsg = "La Publicación requerida no existe";
                CheckStatusCode(response, notFoundMsg);

                return true;

            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return false;
            }
        }

        /// <summary>
        /// API call to update the Status of a Mechanic Publication
        /// </summary>
        /// <param name="pubId"> Publication Id </param>
        /// <param name="newStateId"> Id of the new Status for the Publication </param>
        public bool ChangeStatus(string pubId, string newStateId)
        {
            if (string.IsNullOrEmpty(pubId) || string.IsNullOrEmpty(newStateId))
            {
                ErrorWriter.InvalidArgumentsError();
                return false;
            }

            try
            {
                string url = $"{prefix}/publications/{pubId}/change-status?public_status_id={newStateId}";

                var request = new RestRequest(url, Method.POST);

                var response = client.Execute(request);

                // Throw an exception if the StatusCode is different from 200
                CheckStatusCode(response);

                return true;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return false;
            }
        }

        public bool AddToPubViews(string pubId)
        {
            if (string.IsNullOrEmpty(pubId)) return false;

            try
            {
                var pub = GetPub(pubId);
                if (pub == null) return false;

                pub.views += 1;

                var res = UpdatePub(pub);
                if (!res) return false;

                return true;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return false;
            }
        }

        /* ---------------------------------------------------------------- */
        /* GET SECONDARY DATA */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// API call to get all Publication Status, with name and ID
        /// </summary>
        public IEnumerable<PublicStatus> GetAllStatus()
        {
            try
            {
                var request = new RestRequest($"{prefix}/public-status", Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<List<PublicStatus>>(request);

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