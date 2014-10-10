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
    public class MyFeedListingsController : Controller
    {
        private FeedDBContext db = new FeedDBContext();

        // GET: SingleFeed
        public ActionResult Index(string searchString, string feedTitles)
        {

            var feeds = from f in db.Feeds
                        select f;

           

            var titles = from t in feeds select t.siteName;
            var titlesList = new List<string>();
            titlesList.AddRange(titles);
            ViewBag.feedTitles = new SelectList(titlesList);

            if (!String.IsNullOrEmpty(feedTitles))
            {
                feeds = feeds.Where(x => x.siteName == feedTitles);
            }
            

            List<FeedItem> fil = new List<FeedItem>();
            foreach (Feed site in feeds)
            {
                string siteName = site.siteName;
                string url = site.feedAdress;
                XmlReader reader = XmlReader.Create(url);
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                reader.Close();
                Uri uri;
                
                foreach (SyndicationItem item in feed.Items)
                {
                    FeedItem fi = new FeedItem();
                    fi.siteTitle = siteName;
                    //fi.baseUri = item.BaseUri.OriginalString; Was going to try to link back to the site
                    fi.Title = item.Title.Text;
                    uri = item.Links[0].Uri;
                    fi.address = uri.OriginalString;
                    fi.published = item.PublishDate;
                    fil.Add(fi);
                }

                fil.Sort(delegate(FeedItem x, FeedItem y){
                    return x.CompareTo(y);
                });
                fil.Reverse();
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                IEnumerable<FeedItem> iFil = fil.AsEnumerable<FeedItem>();
                fil = new List<FeedItem>(iFil.Where(s => s.Title.Contains(searchString)));
            }
            return View(fil);
        }
    }
}