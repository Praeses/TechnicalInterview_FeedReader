using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.XPath;
using System.ServiceModel.Syndication;
using System.Data.Entity.Validation;

using FeedReader.Models;
using FeedReader.Providers;
using FeedReader.DBContexts;


using System.Text.RegularExpressions;

namespace FeedReader.Controllers
{
    public class RssController : Controller
    {
        // GET: Rss
        public ActionResult Index(string feedUrl, Dictionary<String, Object> model)
        {
            IRssUpdater updater = new RssUpdater();
            RssChannel channel = updater.retrieveChannel(feedUrl);

            model["channel"] = channel;

            return View(model);
        }

        public ActionResult ListFeeds()
        {
            RssContext context = new RssContext();

            List<RssChannel> feeds = context.RssChannels.ToList();

            return View(feeds);
        }

        public ActionResult ViewFeed(int id)
        {
            RssContext context = new RssContext();
            RssChannel channel = context.RssChannels.Include("Items").Where(itemId => itemId.RssChannelId == id).FirstOrDefault();

            IRssUpdater updater = new RssUpdater();
            channel = updater.retrieveChannel(channel.FeedUrl);

            return View(channel);
        }

        public ActionResult AddFeed()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddFeed(RssChannel channel)
        {
            String link = channel.Link;
            RssUpdater updater = new RssUpdater();
            
            RssChannel retrievedChannel = updater.retrieveChannel(link);

            RssContext context = new RssContext();
            context.RssChannels.Add(retrievedChannel);
            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }


            return RedirectToAction("ListFeeds");
        }

        public ActionResult RemoveFeed()
        {
            return View();
        }

    }
}