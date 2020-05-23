using RestSharp;
using System;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Net;
using jaramillo.cl.Common;

namespace jaramillo.cl.APICallers
{
    public class CallerBase
    {
        public readonly RestClient client;
        private readonly string _url = ConfigurationManager.AppSettings["BuffetAPI.url"];

        public CallerBase()
        {
            client = new RestClient(_url);
            // Get user claims    
            var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
            // Extract the token from there
            var token = claims?.FirstOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/authentication", StringComparison.OrdinalIgnoreCase))?.Value;
            // Add it as a default value in all headers created by this client
            client.AddDefaultHeader("token", token);
        }

        public bool? CheckStatusCode(IRestResponse response, string notFoundMsg = null, string badRequestMsg = null, string unauthorizedMsg = null, string internalServerErrorMsg = null, string genericMgs = null)
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                ErrorWriter.CustomError($"RES CONTENT : {response.Content} \nSTATUS ID : {response.StatusCode}");
            }


            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    throw new Exception(badRequestMsg ?? Resources.Messages.APIError_BadRequest);
                case HttpStatusCode.Unauthorized:
                    throw new Exception(unauthorizedMsg ?? Resources.Messages.APIError_Unauthorized);
                case HttpStatusCode.InternalServerError:
                    throw new Exception(internalServerErrorMsg ?? Resources.Messages.APIError_InternalServerError);
                case HttpStatusCode.NotFound:
                    throw new Exception(notFoundMsg ?? Resources.Messages.APIError_NotFound);
                case HttpStatusCode.OK:
                    return true;
                default:
                    throw new Exception(genericMgs ?? Resources.Messages.Error_SolicitudFallida);
            }

        }
    }
}