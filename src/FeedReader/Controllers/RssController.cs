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
            string userId = User.Identity.GetUserId();

            ICollection<RssSubscription> subscriptions = dbContext.RssSubscriptions.Where(a=>a.UserId == userId).Include(a=>a.Feed).Include("Feed.Items").ToList();

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
        public virtual JsonResult FeedJson(DTableRequest dTableRequest, bool refresh = false)
        {
            RssContext dbContext = new RssContext();

            string userId = User.Identity.GetUserId();
            
            ICollection<RssSubscription> subscriptions = dbContext.RssSubscriptions.Where(a => a.UserId == userId).Include(a => a.Feed).ToList();

            List<RssChannel> feeds = new List<RssChannel>();

            foreach (RssSubscription sub in subscriptions)
            {
                feeds.Add(sub.Feed);
            }

            if (refresh)
            {
                IRssUpdater updater = new RssUpdater();

                foreach (RssChannel channel in feeds)
                {
                    RssChannel updatedChannel = updater.retrieveChannel(channel.FeedUrl);
                    updatedChannel.RssChannelId = channel.RssChannelId;

                    dbContext.Entry(channel).CurrentValues.SetValues(updatedChannel);

                    ICollection<RssItem> itemsToBeAdded = new List<RssItem>();
                    foreach (RssItem updatedItem in updatedChannel.Items)
                    {
                        RssItem existingItem = dbContext.RssItems.Where(a => a.RssChannelId == channel.RssChannelId).Where(a => a.Title == updatedItem.Title).FirstOrDefault();
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

                }
                dbContext.SaveChanges();
            }


            var entries = dbContext.RssItems
                .Join(
                dbContext.RssChannels,
                rssItem => rssItem.RssChannelId,
                rssChannel => rssChannel.RssChannelId,
                (rssItem, rssChannel) => new { rssItem, rssChannel })
                .Join(
                    dbContext.RssSubscriptions,
                    combinedEntry => combinedEntry.rssChannel.RssChannelId,
                    rssSubscription => rssSubscription.RssChannelId,
                    (combinedEntry, rssSubscription) => new
                    {
                        rssChannel = combinedEntry.rssChannel,
                        rssItem = combinedEntry.rssItem,
                        rssSubscription = rssSubscription
                    })
                .Join(
                    dbContext.Users,
                    combined => combined.rssSubscription.UserId,
                    user => user.Id,
                    (combinedEntry, user) => new
                    {
                        rssChannel = combinedEntry.rssChannel,
                        rssItem = combinedEntry.rssItem,
                        rssSubscription = combinedEntry.rssSubscription,
                        user = user
                    })
                  .Where(fullEntry => fullEntry.user.Id == userId)
                  .OrderBy(fullEntry => fullEntry.rssItem.PubDate)
                  .Select(result => new
                  {
                      result.rssItem,
                      filteredChannel = new
                      {
                          Title = result.rssChannel.Title,
                          FeedUrl = result.rssChannel.FeedUrl,
                          Link = result.rssChannel.Link
                      },
                  });

            int totalCount = entries.Count();

            entries.Skip(dTableRequest.Start).Take(dTableRequest.Length).ToList();
            
            IEnumerable<Object> data = entries.Select(e => new
            {
                wrapper = new {
                    title = e.rssItem.Title,
                    pubDate = e.rssItem.PubDate,
                    link = e.rssItem.Link,
                    rssItemId = e.rssItem.RssItemId,
                    channel = new
                    {
                        feedUrl = e.filteredChannel.FeedUrl,
                        title = e.filteredChannel.Title,
                        link = e.filteredChannel.Link
                    }
                }
               
            }).ToList();

            DTableResponse<Object> dTableResponse = new DTableResponse<Object>(new Collection<Object>(data.ToList()));
            dTableResponse.recordsFiltered = data.Count();
            dTableResponse.recordsTotal = totalCount;
            dTableResponse.draw = dTableRequest.Draw;

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