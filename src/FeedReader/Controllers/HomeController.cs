using FeedReader.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using FeedReader.Models.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using FeedReader.Models.Helpers;

namespace FeedReader.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {    
            return View();
        }

        // Method retrieves the users rss feeds
        [HttpGet]
        public ActionResult GetUserRssFeeds(bool update, string search, string filter, int page)
        {
            string userId = User.Identity.GetUserId();

            // Called if we need to update the rss feeds for this user from the url
            if (update)
                UpdateUserFeeds(userId);

            RssFeedDataHelper feedDataHelper = new RssFeedDataHelper();
            List<RssFeedItem> rssFeedItems = feedDataHelper.retrieveRssFeedItemsForUser(userId, search, filter, page);            

            return Json(rssFeedItems, JsonRequestBehavior.AllowGet);
        }

        // Method updates the users rss feeds
        public void UpdateUserFeeds(string userId)
        {
            RssFeedDataHelper feedDataHelper = new RssFeedDataHelper();
            feedDataHelper.updateRssFeedsForUser(userId);
        }

        // Method retrieves the feeds for the user
        [HttpGet]
        public ActionResult RetrieveFeeds()
        {
            RssFeedDataHelper feedDataHelper = new RssFeedDataHelper();
            List<RssFeed> rssFeeds = feedDataHelper.retrieveRssFeedsForUser(User.Identity.GetUserId());

            return Json(rssFeeds, JsonRequestBehavior.AllowGet);
        }

        // Method adds new/existing rss feeds to user
        [HttpPost]
        public ActionResult AddRssFeed(string rssFeedUrl)
        {
            RssFeedDataHelper feedDataHelper = new RssFeedDataHelper();

            String message = "";
            bool error = false;

            try
            {
                string userId = User.Identity.GetUserId();
                RssFeed rssFeed = feedDataHelper.retrieveRssFeed(rssFeedUrl);

                // If null it means this is a feed we haven't saved before in the application
                if (rssFeed == null)
                {
                    // Retrieve rss feed from url and save to database
                    RssFeedService rssFeedService = new RssFeedService();
                    rssFeed = rssFeedService.RetrieveRssFeed(rssFeedUrl);
                    feedDataHelper.saveRssFeed(rssFeed);
                }

                UserRssFeed userRssFeed = feedDataHelper.retireveUserRssFeed(rssFeed.RssFeedId, userId);

                // If UserRssFeed is null we need to create a new one and save to database
                if (userRssFeed == null)
                {
                    // save userRssFeed to database           
                    userRssFeed = new UserRssFeed
                    {
                        UserId = userId,
                        RssFeedId = rssFeed.RssFeedId
                    };
                    feedDataHelper.saveUserRssFeed(userRssFeed);

                    message = String.Format("Successfully added RSS Feed: \"{0}\".", rssFeed.Title);
                }
                else //This means they have already added the Rss Feed
                {
                    error = true;
                    message = String.Format("You have already added RSS Feed: \"{0}\".", rssFeed.Title);
                }                
            }
            catch (System.IO.FileNotFoundException)
            {
                error = true;
                message = String.Format("Unable to locate URL: \"{0}\".  Make sure this is a valid URL.", rssFeedUrl);
            }
            catch
            {
                error = true;
                message = String.Format("Unable to parse feed for URL: \"{0}\".  Make sure this is a valid RSS feed.", rssFeedUrl);
            }

            return Json(new {error=error, message=message});
        }

        // Method removes the rss feed from the user
        [HttpPost]
        public ActionResult RemoveRssFeed(string rssFeedId)
        {
            RssFeedDataHelper feedDataHelper = new RssFeedDataHelper();
            String message = "Successfully removed RSS Feed";
            bool error = false;

            feedDataHelper.removeUserRssFeed(User.Identity.GetUserId(), rssFeedId);                  

            return Json(new {error=error, message=message});
        }
    }
}