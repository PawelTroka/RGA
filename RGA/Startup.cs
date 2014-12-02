using Microsoft.Owin;
using Owin;
using RGA;

[assembly: OwinStartup(typeof (Startup))]

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