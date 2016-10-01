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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;

using System.Data.Entity;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Data.Entity.Infrastructure;

namespace FeedReader.Controllers
{
    [Authorize]
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

        public ActionResult ListFeedsStatic()
        {
            RssContext dbContext = new RssContext();
            RssProvider provider = new RssProvider(dbContext);

            string userId = User.Identity.GetUserId();

            ICollection<RssSubscription> subscriptions = provider.retrieveSubscriptions(userId);

            List<RssChannel> feeds = new List<RssChannel>();
            
            foreach(RssSubscription sub in subscriptions){
                feeds.Add(sub.Feed);
            }

            IRssUpdater updater = new RssUpdater();

            foreach (RssChannel channel in feeds)
            {
                RssChannel updatedChannel = updater.retrieveChannel(channel.FeedUrl);
                updatedChannel.RssChannelId = channel.RssChannelId;

                dbContext.Entry(channel).CurrentValues.SetValues(updatedChannel);

                ICollection<RssItem> itemsToBeAdded = new List<RssItem>();
                foreach (RssItem updatedItem in updatedChannel.Items)
                {
                    RssItem existingItem = channel.Items.FirstOrDefault(item => item.Title == updatedItem.Title);
                    if (existingItem == null)
                    {
                        itemsToBeAdded.Add(updatedItem);
                    }
                    else {
                         //update existing or leave?
                    }
                }
                foreach(RssItem itemToBeAdded in itemsToBeAdded){
                    channel.Items.Add(itemToBeAdded);
                }
                
            }
            dbContext.SaveChanges();

            List<RssItem> items = new List<RssItem>();

            foreach(RssChannel feed in feeds){
                items.AddRange(feed.Items);
            }

            items.Sort((a, b) => b.PubDate.CompareTo(a.PubDate));

            ViewData["feeds"] = feeds;
            ViewData["items"] = items;
            
            return View();
        }

        public ActionResult ListFeeds()
        {
            return View();
        }

        [HttpPost]
        public JsonResult UpdateFeeds()
        {
            RssContext dbContext = new RssContext();
            RssProvider provider = new RssProvider(dbContext);

            string userId = User.Identity.GetUserId();
            ICollection<RssSubscription> subscriptions = provider.retrieveSubscriptions(userId);

            List<RssChannel> feeds = new List<RssChannel>();

            foreach (RssSubscription sub in subscriptions)
            {
                feeds.Add(sub.Feed);
            }

            IRssUpdater updater = new RssUpdater();

            foreach (RssChannel channel in feeds)
            {
                byte[] rowVersion = channel.RowVersion;
                RssChannel updatedChannel = updater.retrieveChannel(channel.FeedUrl);
                updatedChannel.RssChannelId = channel.RssChannelId;

                dbContext.Entry(channel).CurrentValues.SetValues(updatedChannel);
                dbContext.Entry(channel).OriginalValues["RowVersion"] = rowVersion;

                ICollection<RssItem> itemsToBeAdded = new List<RssItem>();
                foreach (RssItem updatedItem in updatedChannel.Items)
                {
                    RssItem existingItem = dbContext.RssItems.Where(a=>a.Hash == updatedItem.Hash).FirstOrDefault();
                    if (existingItem == null)
                    {
                        itemsToBeAdded.Add(updatedItem);
                    }
                    else
                    {
                        //update existing or leave?
                    }
                }
                foreach (RssItem itemToBeAdded in itemsToBeAdded)
                {
                    channel.Items.Add(itemToBeAdded);
                }

                try
                {
                    dbContext.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Debug.WriteLine("RssChannel Update was attempted on an updating channel. Skip as it has been updated");
                }
            }
          

            return Json("success");
        }
        [HttpPost]
        public virtual JsonResult FeedJson(DTableRequest dTableRequest, bool refresh = false)
        {
            RssProvider provider = new RssProvider(new RssContext());   
            string userId = User.Identity.GetUserId();

            dTableRequest.search.value = Request.Params["search[value]"]; //TO-DO: find out why second level objects are not parsed by mvc

            DTableResponse<Object> dTableResponse = provider.retrieveDataTableRssItems(userId, dTableRequest);

            return Json(dTableResponse, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewFeed(string feedUrl)
        {
            RssContext context = new RssContext();
            RssChannel channel = context.RssChannels.Include("Items").Where(itemId => itemId.FeedUrl == feedUrl).FirstOrDefault();

            IRssUpdater updater = new RssUpdater();
            channel = updater.retrieveChannel(feedUrl);

            return View(channel);
        }

        public ActionResult ViewFeedItem(int feedItemId)
        {
            RssContext context = new RssContext();
            RssItem rssItem = context.RssItems.FirstOrDefault(a=> a.RssItemId == feedItemId);

            return PartialView("~/Views/Rss/Partial/ViewFeedItem.cshtml", rssItem);
        }

        public ActionResult AddFeed()
        {
            return View();
        }

        public ActionResult SearchRssFeeds(string query)
        {
            IRssSearchProvider rssSearch = new FeedlyRssProvider();
            return Content(rssSearch.search(query));
        }

        [HttpPost]
        public ActionResult AddFeed(RssChannel channel)
        {
            String link = channel.Link;
            RssUpdater updater = new RssUpdater();
            
            RssChannel retrievedChannel = updater.retrieveChannel(link);

            try
            {
                ApplicationDbContext dbContext = new ApplicationDbContext();
                ApplicationUser appUser = dbContext.Users.Find(User.Identity.GetUserId());

                RssSubscription subscription = new RssSubscription();
                subscription.Feed = retrievedChannel;

                appUser.RssSubscriptions.Add(subscription);
                dbContext.SaveChanges();

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