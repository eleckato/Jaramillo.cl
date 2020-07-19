using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using jaramillo.cl.Models;
using jaramillo.cl.Models.APIModels;
using jaramillo.cl.APICallers;
using jaramillo.cl.Common;
using System.Web.Routing;

namespace jaramillo.cl.Controllers
{
    public class AccountController : BaseController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        // GET: /Account/Login
        [AnonymousOnly]
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        // POST: /Account/Login
        [HttpPost]
        [AnonymousOnly]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            /*
             * No tenemos suficientes SignInStatus así que voy a usar estos dos para otra cosa
             * LockedOut                Invalid Username
             * RequiresVerification     Invalid Credentials
             */
            SignInStatus result = await SignInManager.PasswordSignInAsync(model.LoginUsername, model.LoginPassword, model.RememberMe, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);

                case SignInStatus.LockedOut:
                    SetErrorMsg("El nombre de usuario no existe.");
                    return View(model);

                case SignInStatus.RequiresVerification:
                    SetErrorMsg("La contraseña ingresada no es correcta.");
                    return View(model);

                default:
                    SetErrorMsg("Hubo un error procesando su solicitud, por favor inténtelo nuevamente o pónganse en contacto con soporte.");
                    return View(model);
            }
        }


        // POST: /Account/LogOff
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }


        // GET: /Account/ForgotPassword
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AnonymousOnly]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (model == null)
            {
                Error_FailedRequest();
                return RedirectToAction("Login");
            }

            if (string.IsNullOrEmpty(model.Email))
                return Error_InvalidForm(false);

            try
            {

                var res = new UsuariosCaller().ResetPassword(model.Email);

            }
            catch (Exception e)
            {
                return Error_CustomError(e.Message, false);
            }

            return View("ForgotPasswordConfirmation");
        }


        // GET: /Account/ForgotPasswordConfirmation
        [AnonymousOnly]
        [HttpGet]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }


        //? GET: /Account/ConfirmEmail
        [AnonymousOnly]
        [HttpGet]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }



        // POST: /Account/RegisterClient
        [AnonymousOnly]
        [HttpGet]
        public ActionResult RegisterClient()
        {
            return View();
        }


        // POST: /Account/RegisterClient
        [HttpPost]
        [AnonymousOnly]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterClient(Usuario newUser)
        {
            if (newUser == null) return Error_InvalidUrl();

            string userId;

            try
            {
                var UP = new UsuariosCaller();

                newUser.status_id = "ACT";
                newUser.user_type_id = "CLI";
                newUser.mail_confirmed = false;
                newUser.updated_at = DateTime.Now;
                newUser.deleted = false;
                newUser.appuser_id = Guid.NewGuid().ToString();

                userId = UP.RegisterUser(newUser);

                if (userId == null) return Error_FailedRequest();


            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                Error_CustomError(e.Message);
                return RedirectToAction("RegisterClient");
            }

            string successMsg = "Su cuenta ha sido registrada con éxito!";
            SetSuccessMsg(successMsg);
            TempData["registerSuccess"] = true;

            return RedirectToAction("Login", new { userId });
        }


        // POST: /Account/RegisterClient
        [AnonymousOnly]
        [HttpGet]
        public ActionResult RegisterMech()
        {
            return View();
        }

        // POST: /Account/RegisterClient
        [HttpPost]
        [AnonymousOnly]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterMech(Usuario newUser)
        {
            if (newUser == null) return Error_InvalidUrl();

            string userId;

            try
            {
                var UP = new UsuariosCaller();

                newUser.status_id = "ACT";
                newUser.user_type_id = "MEC";
                newUser.mail_confirmed = false;
                newUser.updated_at = DateTime.Now;
                newUser.deleted = false;
                newUser.appuser_id = Guid.NewGuid().ToString();

                userId = UP.RegisterUser(newUser);

                if (userId == null) return Error_FailedRequest();
                
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                Error_CustomError(e.Message);
                return RedirectToAction("RegisterMech");
            }

            string successMsg = "Su cuenta ha sido registrada con éxito!";
            SetSuccessMsg(successMsg);
            TempData["registerSuccess"] = true;

            return RedirectToAction("Login", new { userId });
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }

    public class AnonymousOnly : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index", area = "" }));
            }
        }
    }
}