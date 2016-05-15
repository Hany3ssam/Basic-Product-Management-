using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PearlDev.Startup))]
namespace PearlDev
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
