using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FeedReader.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;

namespace FeedReader
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ApplicationData.AllNewsFeeds = db.NewsFeed.ToList();

                ApplicationData.AllNewsFeeds.Sort();
            }
        }
    }
}
