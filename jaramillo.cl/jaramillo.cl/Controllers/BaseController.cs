using System.Web;
using System.Web.Mvc;

namespace jaramillo.cl.Controllers
{
    public class BaseController : Controller
    {
        public void SetErrorMsg(string errroMessage)
        {
            TempData["ErrorMessage"] = errroMessage;
        }
        public void SetSuccessMsg(string sucessMessage)
        {
            TempData["SuccessMessage"] = sucessMessage;
        }

        /// <summary>
        /// Get the referrer URL of a Request when an error arise to know where to redirect the User wen 
        /// </summary>
        public string GetRefererForError(HttpRequestBase request)
        {
            // Try to get the referrer URL
            string refererUrl = request?.UrlReferrer?.AbsoluteUri;

            // If is null, redirect to Error page
            // Also, if the requested URL is the same as the referrer, then redirect to Error page to avoid an infinite loop
            if (refererUrl == null || request.Url.Equals(refererUrl)) return Url.Action("Error", "Home");

            // If is safe to go back to the referrer URL, redirect there
            return refererUrl;
        }

        /// <summary>
        /// Get the referrer URL of a Request 
        /// </summary>
        public string GetReferer(HttpRequestBase request)
        {
            // Try to get the referrer URL
            string refererUrl = request?.UrlReferrer?.AbsoluteUri;

            // If is null, redirect to Error page
            if (refererUrl == null) return Url.Action("Error", "Home");

            // If is safe to go back to the referrer URL, redirect there
            return refererUrl;
        }

        /// <summary>
        /// When a URL is malformed, lack arguments or have invalid arguments
        /// <para>Set the correct error message in TempData["ErrorMessage"] and return a safe place to redirect the User</para>
        /// </summary>
        public RedirectResult Error_InvalidUrl(bool errorReferer = true)
        {
            SetErrorMsg(Resources.Messages.Error_URLInvalida);

            string referer = errorReferer ? GetRefererForError(Request) : GetReferer(Request);

            return Redirect(referer);
        }
        /// <summary>
        /// When a received form isn't valid
        /// <para>Set the correct error message in TempData["ErrorMessage"] and return a safe place to redirect the User</para>
        /// </summary>
        public RedirectResult Error_InvalidForm(bool errorReferer = true)
        {
            SetErrorMsg(Resources.Messages.Error_FormInvalido);

            string referer = errorReferer ? GetRefererForError(Request) : GetReferer(Request);

            return Redirect(referer);
        }
        /// <summary>
        /// When an unknown error with the request arises
        /// <para>Set the correct error message in TempData["ErrorMessage"] and return a safe place to redirect the User</para>
        /// </summary>
        public RedirectResult Error_FailedRequest(bool errorReferer = true)
        {
            SetErrorMsg(Resources.Messages.Error_SolicitudFallida);

            string referer = errorReferer ? GetRefererForError(Request) : GetReferer(Request);

            return Redirect(referer);
        }
        /// <summary>
        /// When an user try to get into a place they should not be
        /// <para>Set the correct error message in TempData["ErrorMessage"] and return a safe place to redirect the User</para>
        /// </summary>
        public RedirectResult Error_Unauthorized(bool errorReferer = true)
        {
            SetErrorMsg(Resources.Messages.Error_Unauthorized);

            string referer = errorReferer ? GetRefererForError(Request) : GetReferer(Request);

            return Redirect(referer);
        }
        /// <summary>
        /// For custom errors. It the error message on TempData["ErrorMessage"] and return a safe place to redirect the User
        /// </summary>
        /// <param name="error"></param>
        public RedirectResult Error_CustomError(string error, bool errorReferer = true)
        {
            SetErrorMsg(error);

            string referer = errorReferer ? GetRefererForError(Request) : GetReferer(Request);

            return Redirect(referer);
        }
    }

}