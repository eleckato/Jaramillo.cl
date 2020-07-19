using jaramillo.cl.Models.APIModels;
using RestSharp;
using System;
using System.Collections.Generic;
using jaramillo.cl.Common;
using System.Net;
using System.Linq;
using jaramillo.cl.Providers;

namespace jaramillo.cl.APICallers
{
    public class UsuariosCaller : CallerBase
    {
        private readonly string prefix = "user-adm";

        /// <summary>
        /// API call to list all Users
        /// </summary>
        public IEnumerable<Usuario> GetAllUsers(string userName, string userRut, string userTypeId, string userStatusId, bool deleted = false)
        {
            try
            {
                var delString = deleted ? "&deleted=true" : "";
                var url = $"{prefix}/users?username={userName}&rut={userRut}&user_type_id={userTypeId}&status_id={userStatusId}{delString}";

                var request = new RestRequest(url, Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<List<Usuario>>(request);

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
        /// API call to get an User
        /// </summary>
        /// <param name="userId"> User Id </param>
        public Usuario GetUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                ErrorWriter.InvalidArgumentsError();
                return null;
            }

            try
            {
                var request = new RestRequest($"{prefix}/users/{userId}", Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<Usuario>(request);

                string notFoundMsg = "El Usuario requerido no existe";
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
        /// API call to update an User
        /// </summary>
        /// <param name="newUser"> New User </param>
        public bool UpdateUser(Usuario newUser)
        {
            if (newUser == null)
            {
                ErrorWriter.InvalidArgumentsError();
                return false;
            }

            try
            {
                var userId = newUser.appuser_id;

                var request = new RestRequest($"{prefix}/users/{userId}", Method.POST)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddJsonBody(newUser);

                var response = client.Execute(request);

                string notFoundMsg = "El Usuario requerido no existe";
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
        /// API call to change the Status of an User
        /// </summary>
        /// <param name="userId"> User Id </param>
        /// <param name="userStatusId"> User Status Id </param>
        public bool ChangeUserStatus(string userId, string userStatusId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userStatusId))
            {
                ErrorWriter.InvalidArgumentsError();
                return false;
            }

            try
            {
                string url = $"{prefix}/users/{userId}/change-status?status={userStatusId}";

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

        /* ---------------------------------------------------------------- */
        /* GET SECONDARY DATA */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// API call to get all User Types, with name and ID
        /// </summary>
        public IEnumerable<UserType> GetAllTypes()
        {
            try
            {
                var request = new RestRequest($"{prefix}/user-type", Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<List<UserType>>(request);

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
        /// API call to get all User Status, with name and ID
        /// </summary>
        public IEnumerable<UserStatus> GetAllStatus()
        {
            try
            {
                var request = new RestRequest($"{prefix}/user-status", Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<List<UserStatus>>(request);

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
        /// <param name="user"> User to process </param>
        /// <param name="userTypeLst"> List with all User Types </param>
        /// <param name="userStatusLst"> List with all User Status </param>
        public Usuario ProcessUser(Usuario user, List<UserType> userTypeLst, List<UserStatus> userStatusLst)
        {
            if (user == null || userTypeLst == null || userStatusLst == null) return null;

            var thisUserType = userTypeLst.FirstOrDefault(type => type.user_type_id.Equals(user.user_type_id));
            user.user_type_name = thisUserType?.name ?? string.Empty;
            var thisUserStatus = userStatusLst.FirstOrDefault(status => status.status_id.Equals(user.status_id));
            user.status_name = thisUserStatus?.status ?? string.Empty;

            return user;
        }


        /* ---------------------------------------------------------------- */
        /* USER AUTH */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// API call to register an User
        /// </summary>
        /// <param name="newUser"> New User </param>
        public string RegisterUser(Usuario newUser)
        {
            if (newUser == null)
            {
                ErrorWriter.InvalidArgumentsError();
                return null;
            }

            try
            {
                var userId = newUser.appuser_id;

                var request = new RestRequest("user-auth/register", Method.POST)
                {
                    RequestFormat = DataFormat.Json
                };

                newUser.hash = JwtProvider.EncryptHMAC(newUser.hash);
                request.AddJsonBody(newUser);

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

        /// <summary>
        /// API call to change the Password of an User
        /// </summary>
        /// <param name="password"></param>
        /// <param name="oldPassword"></param>
        /// <returns></returns>
        public bool UpdatePassword(string password, string oldPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(oldPassword))
            {
                ErrorWriter.InvalidArgumentsError();
                return false;
            }

            try
            {
                var request = new RestRequest($"user-auth/change-password", Method.POST);

                password = JwtProvider.EncryptHMAC(password);
                oldPassword = JwtProvider.EncryptHMAC(oldPassword);
                request.AddJsonBody(new { psw = password, old_psw = oldPassword });

                var response = client.Execute(request);

                if (response.StatusCode == HttpStatusCode.Conflict)
                    throw new Exception("La contraseña ingresada es incorrecta");

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

        /// <summary>
        /// API call that will send an Email to the User with a new password. It doesn't matter if the mail exist or not, 
        /// the request will always return 200 if the connection was successful
        /// </summary>
        /// <param name="email"> User Email </param>
        public bool ResetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ErrorWriter.InvalidArgumentsError();
                return false;
            }

            try
            {
                var request = new RestRequest($"user-auth/Recover-pass?email={email}", Method.POST);

                var response = client.Execute(request);

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