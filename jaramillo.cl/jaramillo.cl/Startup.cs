using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(jaramillo.cl.Startup))]
namespace jaramillo.cl
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
