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

        // TODO Pagination
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

                var pubStatusList = GetAllStatus().ToList();
                if (pubStatusList == null) return null;

                List<Mecanico> mechList = new MecanicosCaller().GetAllMech(null, null, null).ToList();

                pubList.ForEach(x => {
                    x = ProcessPub(x, pubStatusList, mechList);
                });

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

                var pubStatusList = GetAllStatus().ToList();
                if (pubStatusList == null) return null;

                List<Mecanico> mechList = new MecanicosCaller().GetAllMech(null, null, null).ToList();

                pub = ProcessPub(pub, pubStatusList, mechList);

                return pub;
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


        /* ---------------------------------------------------------------- */
        /* HELPERS */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// Set all the secondary data ,like getting the status Name from the status Id
        /// </summary>
        /// <param name="pub"> Publication to process </param>
        /// <param name="pubStatusLst"> List with all Status Names </param>
        public PublicacionMec ProcessPub(PublicacionMec pub, List<PublicStatus> pubStatusLst, List<Mecanico> mechList)
        {
            if (pub == null || pubStatusLst == null) return null;

            var thisUserType = pubStatusLst.FirstOrDefault(type => type.public_status_id.Equals(pub.public_status_id));
            pub.status_name = thisUserType?.status_name ?? string.Empty;

            var thisMech = mechList.FirstOrDefault(x => x.appuser_id.Equals(pub.appuser_id));
            pub.mech_name = thisMech?.fullName ?? "Usuario Eliminado";

            return pub;
        }

    }
}