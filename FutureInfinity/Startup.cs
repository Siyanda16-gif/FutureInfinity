using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GROUP_Q.Startup))]
namespace GROUP_Q
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
