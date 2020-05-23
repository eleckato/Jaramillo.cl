using System.Web;
using System.Web.Optimization;

namespace jaramillo.cl
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/umd/popper.js",
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/ContentCSS").Include(
                      "~/Content/bootstrap.min.css",  // Base Bootstrap styles
                      "~/Content/fontawesome.min.css",  // Base FontAwesome icons
                      "~/Content/solid.min.css",  // Solid FontAwesome icons
                      "~/Content/owl.carousel.css",  // Owl Carousel
                      "~/Content/Site.css" // Site-wide styles
                      ));

            // Site-wide Scripts
            bundles.Add(new ScriptBundle("~/MainScripts").Include(
                        "~/Scripts/js.cookie.min.js",   // Library to deal with cookies easily from JS
                        "~/Scripts/owl.carousel.min.js",   // Owl Carousel
                        "~/Content/JS/common.js",   // Common utilities and functions
                        "~/Content/JS/_layout.js"   // JS for _Layout.cshtml
                        ));

            bundles.Add(new ScriptBundle("~/HeadScrips").Include(
                    "~/Content/JS/head.util.js"
                ));
        }
    }
}
