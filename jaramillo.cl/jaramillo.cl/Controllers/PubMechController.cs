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
    [Authorize(Roles = "MEC")]
    public class PubMechController : BaseController
    {
        readonly PublicacionesMecCaller PC = new PublicacionesMecCaller();
        readonly UsuariosCaller UC = new UsuariosCaller();

        /* ---------------------------------------------------------------- */
        /* PUBLICATION LIST */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// GET  |  Return a view with a list of Publications
        /// </summary>
        [HttpGet]
        public ActionResult PubList()
        {
            List<PublicacionMec> pubs;

            try
            {
                // Ger user data
                var userId = User.Identity.GetUserId();
                var user = UC.GetUser(userId);
                if (user == null) return Error_FailedRequest();

                // Conseguir todas las publicaciones porque obvio no hay filtro de user por API :tired_af:
                pubs = PC.GetAllPub(string.Empty, string.Empty, string.Empty, string.Empty).ToList();
                if (pubs == null) return Error_FailedRequest();

                // Filtrar para las publicaciones de Mecánicos
                pubs = pubs.Where(x => x.appuser_id.Equals(userId)).ToList();

            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            return View(pubs);
        }


        /* ---------------------------------------------------------------- */
        /* ADD PUBLICATION */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// GET  |  Returns a form to request a Publication
        /// </summary>
        [HttpGet]
        public ActionResult AddPub()
        {
            PublicacionMec pub;

            try
            {
                // Get userId and the user to check that is actually a Mechanic, just to be sure
                var userId = User.Identity.GetUserId();
                var user = UC.GetUser(userId);
                if (user == null) return Error_FailedRequest();

                if (!user.user_type_id.Equals("MEC")) return Error_Unauthorized();
                if (!user.mail_confirmed) return Error_CustomError("Debes confirmar tu mail antes de enviar una solicitud de Publicación!");

                // Make templates
                pub = new PublicacionMec(true)
                {
                    appuser_id = userId,
                    user_type_id = user.user_type_id,
                    email = user.email,
                };
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            return View(pub);
        }

        /// <summary>
        /// POST  |  Send the Publication requested to the API to save it in the DB as a Pending Publication
        /// </summary>
        /// <param name="newPub">Model with the new Publication</param>
        [HttpPost]
        public ActionResult AddPub(PublicacionMec newPub)
        {
            if (newPub == null) return Error_InvalidUrl();

            string newPubId;

            try
            {
                newPub.created_at = DateTime.Now;
                newPub.updated_at = DateTime.Now;

                newPubId = PC.AddPub(newPub);
                if (newPubId == null) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            SetSuccessMsg("Publicación creada con éxito, te vamos a mandar un mail cuando sea aceptada por nuestro personal!");
            return RedirectToAction("PubDetails", new { pubId = newPubId });
        }


        /* ---------------------------------------------------------------- */
        /* PUBLICATION DETAILS */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// GET  |  Show all the data of a Publication
        /// <param name="pubId"> Publication Id </param>
        /// </summary>
        [HttpGet]
        public ActionResult PubDetails(string pubId)
        {
            if (string.IsNullOrEmpty(pubId)) return Error_InvalidUrl();

            PublicacionMec pub;

            try
            {
                pub = PC.GetPub(pubId);
                if (pub == null) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            return View(pub);
        }


        /* ---------------------------------------------------------------- */
        /* UPDATE PUBLICATION */
        /* ---------------------------------------------------------------- */
        /// <summary>
        /// GET |   Returns a form to update a Publication 
        /// </summary>
        /// <param name="pubId">Id of the Publication to update</param>
        [HttpGet]
        public ActionResult UpdatePub(string pubId)
        {
            if (string.IsNullOrEmpty(pubId)) return Error_InvalidUrl();

            PublicacionMec pub;

            try
            {
                pub = PC.GetPub(pubId);
                if (pub == null) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            return View(pub);
        }

        /// <summary>
        /// POST  |  Send the Publication requested to the API to update the Publication in the DB
        /// </summary>
        /// <param name="model">Model with the new data</param>

        [HttpPost]
        public ActionResult UpdatePub(PublicacionMec model)
        {
            if (model == null) return Error_InvalidUrl();

            try
            {
                var newPub = PC.GetPub(model.public_id);
                if (newPub == null) return Error_FailedRequest();

                newPub.title = model.title;
                newPub.public_desc = model.public_desc;
                newPub.schedule = model.schedule;
                newPub.services = model.services;
                newPub.bussiness_name = model.bussiness_name;
                newPub.address = model.address;
                newPub.comuna = model.comuna;
                newPub.region = model.region;
                newPub.landline = model.landline;
                newPub.mobile_number = model.mobile_number;
                newPub.updated_at = DateTime.Now;

                var res = PC.UpdatePub(newPub);
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            SetSuccessMsg("Publicación actualizada con éxito");
            return RedirectToAction("PubDetails", new { pubId = model.public_id });
        }



        /* ---------------------------------------------------------------- */
        /* CHANGE STATUS */
        /* ---------------------------------------------------------------- */
        /// <summary>
        /// GET |   API Call to update the status of a Publication
        /// </summary>
        /// <param name="pubId">Id of the Publication to update</param>
        /// <param name="newStatusId">Id of the new Status</param>
        [HttpGet]
        public ActionResult ChangePubStatus(string pubId, string newStatusId)
        {
            if (string.IsNullOrEmpty(pubId) || string.IsNullOrEmpty(newStatusId)) return Error_InvalidUrl();

            try
            {
                bool res = PC.ChangeStatus(pubId, newStatusId);
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            return Redirect(GetReferer(Request));
        }

        /* ---------------------------------------------------------------- */
        /* PAY PUBLICATION */
        /* ---------------------------------------------------------------- */
        /// <summary>
        /// GET |   Returns a View to Pay for a Publication
        /// </summary>
        /// <param name="pubId">Id of the Publication to pay</param>
        [HttpGet]
        public ActionResult PayPub(string pubId)
        {
            if (string.IsNullOrEmpty(pubId)) return Error_InvalidUrl();

            PublicacionMec pub;

            try
            {
                pub = PC.GetPub(pubId);
                if (pub == null) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            return View(pub);
        }

        /// <summary>
        /// POST    |   API call to flag the Publication as payed in the DB
        /// </summary>
        /// <param name="pubId">Id of the Publication to pay</param>
        /// <param name="res">Result of the payment in whatever pay platform its used</param>
        [HttpPost]
        public ActionResult PayPub(string pubId, bool res)
        {
            try
            {
                if (res)
                {
                    var changeRes = PC.ChangeStatus(pubId, "ACT");
                    if (!changeRes) return Error_FailedRequest();
                }
                else
                {
                    SetErrorMsg("Hubo un error procesando su pago, por favor inténtelo nuevamente. Si el problema persiste, contacte a soporte");
                    return RedirectToAction("PubDetails", new { pubId });
                }
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            SetSuccessMsg("La Publicación fue pagada cone éxito");
            return RedirectToAction("PubDetails", new { pubId });
        }

    }
}