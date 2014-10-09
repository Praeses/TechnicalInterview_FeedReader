using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Terradue.ServiceModel.Syndication;


namespace FeedReader.Controllers
{
    public class SingleFeedController : Controller
    {
        // GET: SingleFeed
        public ActionResult Index()
        {
            string url = "http://www.siliconera.com/feed/";
            XmlReader reader = XmlReader.Create(url);
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            reader.Close();
            foreach (SyndicationItem item in feed.Items)
            {
                
            }
            return View();
        }
    }
}