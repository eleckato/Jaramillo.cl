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
        readonly MecanicosCaller MC = new MecanicosCaller();
        readonly UsuariosCaller UC = new UsuariosCaller();


        // PubList
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


        // AddPub


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
                newPub.email = model.email;

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
        /* PAY PUBLICATION */
        /* ---------------------------------------------------------------- */

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

        [HttpPost]
        public ActionResult PayPub(string pubId, bool res)
        {
            try
            {
                if (res)
                {
                    var changeRes = PC.ChangeStatus(pubId, "PEN");
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


        /* ---------------------------------------------------------------- */
        /* OTHER ACTIONS */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// POST  |  API call to update the Status of a Publication
        /// </summary>
        /// <param name="pubId"> Id of the Publication to update </param>
        /// <param name="newStatusId"> Id of the new Status for the Publication </param>
        public ActionResult ChangePubStatus(string pubId, string newStatusId, string msg = null)
        {
            if (string.IsNullOrEmpty(pubId) || string.IsNullOrEmpty(newStatusId)) return Error_InvalidForm();

            try
            {
                var res = PC.ChangeStatus(pubId, newStatusId);
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            string contextMsg;
            switch (newStatusId)
            {
                case "REJ":
                    contextMsg = "La Publicación ha sido rechazada.";
                    break;
                case "DEB":
                    contextMsg = "La Publicación ha sido Aceptada.";
                    break;
                case "ACT":
                    contextMsg = "La Publicación ha sido Activada con éxito.";
                    break;
                case "INA":
                    contextMsg = "La Publicación ha sido Desactivada con éxito.";
                    break;
                default:
                    contextMsg = "El Status de la Publicación ha sido actualizado con Éxito";
                    break;
            }

            SetSuccessMsg(msg ?? contextMsg);

            string referer = GetRefererForError(Request);
            return Redirect(referer);
        }

    }
}