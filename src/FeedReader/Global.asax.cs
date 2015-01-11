using System.Data.Entity;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FeedReader.Infastructure;

namespace FeedReader
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Bootstrapper.Initialise();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Make the database seed itself at startup (if neccessary)
            Database.SetInitializer( new MigrateDatabaseToLatestVersion<FeedReaderDb, Migrations.Configuration>());
            
        }
    }
}
