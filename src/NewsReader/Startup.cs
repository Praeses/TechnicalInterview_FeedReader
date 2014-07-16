using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NewsReader.Startup))]
namespace NewsReader
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
