using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(InitialApp4._5._1.Startup))]
namespace InitialApp4._5._1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
