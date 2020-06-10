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
    [Authorize(Roles = "CLI")]
    public class PublicacionMecController : BaseController
    {
        readonly PublicacionesMecCaller PMC = new PublicacionesMecCaller();
        readonly MecanicosCaller MC = new MecanicosCaller();
        readonly UsuariosCaller UC = new UsuariosCaller();

        /* ---------------------------------------------------------------- */
        /* PUBLICATION LIST */
        /* ---------------------------------------------------------------- */
        [HttpGet]
        public ActionResult PubList(string comuna, string bussName, string pubTitle)
        {
            List<PublicacionMec> pubs;


            try
            {
                pubs = PMC.GetAllPub(comuna, "ACT", bussName, pubTitle).ToList();
                if (pubs == null) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            // To keep the state of the search filters when the user make a search
            ViewBag.comuna = comuna;
            ViewBag.bussName = bussName;
            ViewBag.pubTitle = pubTitle;

            return View(pubs);
        }


        /* ---------------------------------------------------------------- */
        /* PUBLICATION DETAILS */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// GET  |  Show all the data of a Publication
        /// <para> /mecanicos/publicaciones/{id} </para>
        /// </summary>
        [HttpGet]
        public ActionResult PubDetails(string pubId)
        {
            if (string.IsNullOrEmpty(pubId)) return Error_InvalidUrl();

            PublicacionMec pub;
            Usuario user;

            try
            {
                pub = PMC.GetPub(pubId);
                if (pub == null) return Error_FailedRequest();

                var userId = User.Identity.GetUserId();
                user = UC.GetUser(userId);
                if (user == null) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            ViewBag.User = user;

            return View(pub);
        }


        public ActionResult ContactMech(string user, string email, string message)
        {


            SetSuccessMsg("Mensaje Enviado con Éxito");
            return Redirect(GetReferer(Request));
        }

    }
}