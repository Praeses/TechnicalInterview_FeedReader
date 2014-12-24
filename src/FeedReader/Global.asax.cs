using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using log4net;

namespace FeedReader
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public ILog log
        {
            get { return LogManager.GetLogger("myNewsstand"); }
        }
        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config"));
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            log.Fatal("Site Application_Error", Server.GetLastError());
            System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
            Server.ClearError();
        }
    }
}
