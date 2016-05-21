using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PaypalIPN.Startup))]
namespace PaypalIPN
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
