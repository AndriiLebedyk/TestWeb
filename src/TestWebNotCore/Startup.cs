using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TestWebNotCore.Startup))]
namespace TestWebNotCore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
