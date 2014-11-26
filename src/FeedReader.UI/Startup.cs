using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FeedReader.UI.Startup))]
namespace FeedReader.UI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
