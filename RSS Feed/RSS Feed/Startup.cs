using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RSS_Feed.Startup))]
namespace RSS_Feed
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
