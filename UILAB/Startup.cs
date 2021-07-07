using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UILAB.Startup))]
namespace UILAB
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
