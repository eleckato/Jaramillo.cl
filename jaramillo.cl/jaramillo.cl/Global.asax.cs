using jaramillo.cl.Common;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace jaramillo.cl
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            DataAnnotationsModelValidatorProvider.RegisterAdapterFactory(
                typeof(RequiredAttribute),
                (metadata, controllerContext, attribute) => new FixedRequiredAttributeAdapter(metadata, controllerContext, (RequiredAttribute)attribute)
            );

            //set the antiforgery claim to user id
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
        }
    }
}
