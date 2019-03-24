using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Certification_System.Startup))]
namespace Certification_System
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
