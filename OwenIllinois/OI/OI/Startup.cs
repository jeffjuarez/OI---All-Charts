using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OI.Startup))]
namespace OI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
