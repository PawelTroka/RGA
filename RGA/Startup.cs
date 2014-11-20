using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RGA.Startup))]
namespace RGA
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
