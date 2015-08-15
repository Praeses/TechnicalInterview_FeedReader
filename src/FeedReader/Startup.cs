using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FeedReader.Startup))]
namespace FeedReader
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
