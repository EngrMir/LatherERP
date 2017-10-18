using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ERP_Leather.Startup))]
namespace ERP_Leather
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
