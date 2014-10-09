using FeedReader.Models;
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
        private FeedItemsDBContext db = new FeedItemsDBContext();

        // GET: SingleFeed
        public ActionResult Index()
        {
            
            string url = "http://xkcd.com/rss.xml";
            XmlReader reader = XmlReader.Create(url);
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            reader.Close();
            Uri uri;
            List<FeedItem> fil = new List<FeedItem>();
            foreach (SyndicationItem item in feed.Items)
            {
                FeedItem fi = new FeedItem();
                fi.Title = item.Title.Text;
                uri = item.Links[0].Uri;
                fi.address = uri.OriginalString;
                fil.Add(fi);
                    db.FeedItems.Add(fi);


            }
            return View(fil);
        }
    }
}