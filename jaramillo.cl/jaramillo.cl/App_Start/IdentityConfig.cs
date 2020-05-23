using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using jaramillo.cl.Models;
using System.Configuration;
using Newtonsoft.Json.Linq;

namespace jaramillo.cl
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }


        /*
         * No tenemos suficientes SignInStatus así que voy a usar estos dos para otra cosa
         * LockedOut                Invalid Username
         * RequiresVerification     Invalid Credentials
         */
        public override async Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            //// MAKE FAKE CREDENTIALS
            //var claims = new Providers.JwtProvider().CreateFakeIdentity();
            //// SIGN IN
            //HttpContext.Current.Request.GetOwinContext().Authentication.SignIn(claims);
            //// RETURN SUCCESS
            //return SignInStatus.Success;

            // Authorization server end point
            string uri = ConfigurationManager.AppSettings["BuffetAPI.url"];

            var jwtProvider = Providers.JwtProvider.Create(uri);

            string response = await jwtProvider.GetTokenAsync(userName, password);

            if (response == null)
            {
                return SignInStatus.Failure;
            }
            else if (response.StartsWith("ERR"))
            {
                switch (response)
                {
                    // Invalid Username
                    case "ERR-AUTH-001":
                        return SignInStatus.LockedOut;

                    // Invalid Credentials
                    case "ERR-AUTH-002":
                        return SignInStatus.RequiresVerification;

                    default:
                        return SignInStatus.Failure;
                }
            }
            else
            {
                // Get the token!
                string token = (string)JObject.Parse(response)["JWT"]["string"];

                // Decode payload
                dynamic payload = jwtProvider.DecodePayload(response);

                // ERROR: Malformed Token
                if (payload == null) return SignInStatus.Failure;

                // Create an Identity Claim
                ClaimsIdentity claims = jwtProvider.CreateIdentity(true, userName, payload, token);

                // ERROR: Malformed Token
                if (claims == null) return SignInStatus.Failure;

                // Sign in
                var context = HttpContext.Current.Request.GetOwinContext();
                var authenticationManager = context.Authentication;
                authenticationManager.SignIn(claims);

                return SignInStatus.Success;
            }

        }


    }
}
