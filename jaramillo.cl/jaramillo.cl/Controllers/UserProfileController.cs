using jaramillo.cl.APICallers;
using jaramillo.cl.Common;
using jaramillo.cl.Models.APIModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace jaramillo.cl.Controllers
{
    [Authorize]
    [RoutePrefix("perfil")]
    public class UserProfileController : BaseController
    {
        readonly UsuariosCaller UC = new UsuariosCaller();
        public string currentUserId = "";

        private const string updateRoute = "actualizar-datos";
        private const string ChangePasswordUrl = "cambiar-contraseña";

        [HttpGet]
        [Route]
        public ActionResult Profile()
        {
            var userId = User.Identity.GetUserId();
            if (userId == null) return Error_FailedRequest();

            Usuario usuario;

            try
            {
                usuario = UC.GetUser(userId);
                if (usuario == null) return Error_FailedRequest();

                var userTypeLst = UC.GetAllTypes().ToList();
                if (userTypeLst == null) return Error_FailedRequest();

                var userStatusLst = UC.GetAllStatus().ToList();
                if (userStatusLst == null) return Error_FailedRequest();

                usuario = UC.ProcessUser(usuario, userTypeLst, userStatusLst);
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            return View(usuario);
        }


        [HttpGet]
        [Route(updateRoute)]
        public ActionResult UpdateProfile()
        {
            var userId = User.Identity.GetUserId();
            if (userId == null) return Error_FailedRequest();

            Usuario usuario;
            List<UserType> userTypeLst;
            List<UserStatus> userStatusLst;

            try
            {
                usuario = UC.GetUser(userId);
                if (usuario == null) return Error_FailedRequest();

                userTypeLst = UC.GetAllTypes().ToList();
                if (userTypeLst == null) return Error_FailedRequest();

                userStatusLst = UC.GetAllStatus().ToList();
                if (userStatusLst == null) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            ViewBag.userTypeLst = new SelectList(userTypeLst, "user_type_id", "name", usuario.user_type_id);
            ViewBag.userStatusLst = new SelectList(userStatusLst, "status_id", "status", usuario.status_id);

            return View(usuario);
        }

        [HttpPost]
        [Route(updateRoute)]
        public ActionResult UpdateProfile(Usuario newUser)
        {
            if (newUser == null) return Error_InvalidUrl();

            try
            {
                Usuario oldUser = UC.GetUser(newUser.appuser_id);
                if (oldUser == null) return Error_FailedRequest();

                oldUser.name = newUser.name;
                oldUser.last_names = newUser.last_names;
                oldUser.rut = newUser.rut;
                oldUser.adress = newUser.adress;
                oldUser.phone = newUser.phone;
                oldUser.birthday = newUser.birthday;

                var res = UC.UpdateUser(oldUser);
                if (!res)
                {
                    Error_FailedRequest();
                    return RedirectToAction("UpdateProfile", new { userId = newUser.appuser_id });
                }
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                Error_CustomError(e.Message);
                return RedirectToAction("UpdateProfile", new { userId = newUser.appuser_id });
            }

            string successMsg = "Su perfil fue actualizado con éxito";
            SetSuccessMsg(successMsg);

            return RedirectToAction("Profile");
        }



        [HttpGet]
        [Route(ChangePasswordUrl)]
        public ActionResult ChangePassword()
        {
            // Get current userId
            var userId = User.Identity.GetUserId();
            if (userId == null) return Error_FailedRequest();

            return View();
        }


        [HttpPost]
        [Route(ChangePasswordUrl)]
        public ActionResult ChangePassword(string newPsw, string newPsw2, string oldPsw, string oldPsw2)
        {
            if (string.IsNullOrEmpty(newPsw) || string.IsNullOrEmpty(newPsw2) ||
                string.IsNullOrEmpty(oldPsw) || string.IsNullOrEmpty(oldPsw2)) 
                    return Error_InvalidForm(false);

            if (!oldPsw.Equals(oldPsw2) || !newPsw.Equals(newPsw2)) 
                return Error_CustomError("Las contraseñas ingresadas no coinciden", false);

            try
            {
                var res = UC.UpdatePassword(newPsw, oldPsw);
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message, false);
            }

            string successMsg = "Su contraseña fue cambiada";
            SetSuccessMsg(successMsg);

            return RedirectToAction("Profile");
        }
    }
}