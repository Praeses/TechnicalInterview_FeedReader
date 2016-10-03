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
        
        /// <summary>
        /// Index of RSS. Take user to Feed List on first pass
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return RedirectToAction("ListAllItems");
        }

        public ActionResult ListAllFeeds()
        {
            RssManager manager = new RssManager(User.Identity.GetUserId());
            var query = from sub in manager.RetrieveSubscriptions()
                        orderby sub.Feed.Title.Trim()
                        select sub.Feed;
      
            return View(query.ToList());
        }
        /// <summary>
        /// Takes user to the non static feed list that is updated with JSON
        /// </summary>
        /// <param name="refreshFeed">Tells the front end to make a call to the UpdateFeeds method on load</param>
        /// <returns></returns>
        public ActionResult ListAllItems(bool refreshFeed = true)
        {
            ViewData["refreshFeed"] = refreshFeed;

            return View();
        }

        /// <summary>
        /// Make a request to the update manager to update the user's feeds.
        /// </summary>
        /// <returns>"success" if the action succeeded</returns>
        [HttpPost]
        public JsonResult UpdateFeeds()
        {
            RssManager manager = new RssManager(User.Identity.GetUserId());
            manager.RequestChannelUpdate();

            return Json("success");
        }
        /// <summary>
        /// Remove an RssSubscription from the user's feeds.
        /// </summary>
        /// <param name="rssChannelId">The Id to remove the subscription of</param>
        /// <returns></returns>
        public ActionResult RemoveSubscription(int rssChannelId)
        {
            RssManager manager = new RssManager(User.Identity.GetUserId());
            manager.RemoveSubscription(rssChannelId);
            return RedirectToAction("ListAllItems");
        }

        /// <summary>
        /// Updates the feed item that a user clicks on while on the full feed list.
        /// </summary>
        /// <param name="rssItemId">ID of the RssItem clicked</param>
        /// <param name="hide">Prevents the feed item from being pulled back in again</param>
        /// <returns>string result of "success" if the operation succeeded.</returns>
        [HttpPost]
        public JsonResult UpdateFeedItem(int rssItemId, bool hide = false)
        {
            string userId = User.Identity.GetUserId();
            RssManager manager = new RssManager(userId);
            manager.UpdateRssItem(rssItemId, hide,true);
          
            return Json("success");
        }

        /// <summary>
        /// Returns a JSON representation of the feeds requested for the user in a format that the front end DataTables plugin will understand
        /// </summary>
        /// <param name="dTableRequest">Object representing the request the Datatables plugin made to the controller.</param>
        /// <returns>JSON representation of the DTableResponse object</returns>
        [HttpPost]
        public virtual JsonResult FeedJson(DTableRequest dTableRequest)
        {
            RssManager manager = new RssManager(User.Identity.GetUserId());   
            string userId = User.Identity.GetUserId();

            dTableRequest.search.value = Request.Params["search[value]"]; //TO-DO: find out why second level objects are not parsed by mvc

            DTableResponse<Object> dTableResponse = manager.RetrieveDataTableRssItems(dTableRequest);

            return Json(dTableResponse, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// View a specific URL feed by passing the the feedURL of the channel requested. The url is used instead of channel id so that the method will be able to handle feeds
        /// that the user might not yet be subscribed to
        /// </summary>
        /// <param name="feedUrl">Url of the feed requested</param>
        /// <returns>Page representation of the view requested</returns>
        public ActionResult ViewFeed(string feedUrl)
        {
            RssContext context = new RssContext();
            RssChannel channel = context.RssChannels.Include("Items").Where(itemId => itemId.FeedUrl == feedUrl).FirstOrDefault();
            channel.Items = new List<RssItem>(channel.Items.OrderByDescending(a => a.PubDate));

            return View(channel);
        }

        /// <summary>
        /// Method used to return just a partial view of the feed item requested. Can be used on a page where part of the screen needs to be populated with the rssItem requested
        /// </summary>
        /// <param name="feedItemId">Rss item id to view</param>
        /// <returns>Partial View of the feed item</returns>
        public ActionResult ViewFeedItem(int feedItemId)
        {
            RssContext context = new RssContext();
            RssItem rssItem = context.RssItems.Include(a=>a.Channel).FirstOrDefault(a=> a.RssItemId == feedItemId);

            return PartialView("~/Views/Rss/Partial/ViewFeedItem.cshtml", rssItem);
        }

        /// <summary>
        /// Gives the ability to search for an rss feed rather than the user having to know the specific url of the feed requested.
        /// </summary>
        /// <param name="query">query string of the search</param>
        /// <returns>View representing the search result</returns>
        public ActionResult SearchRssFeeds(string query)
        {
            IRssSearchProvider rssSearch = new FeedlyRssProvider();
            return Content(rssSearch.search(query));
        }

        /// <summary>
        /// Controller entry to add a new Rss subscription to the user's subscription list
        /// </summary>
        /// <returns>Page view to add a new subscription</returns>
        public ActionResult AddFeed()
        {
            return View();
        }

        /// <summary>
        /// Post method of the Add Feed page. Will add a new subscription to the user's account if it passes validation
        /// </summary>
        /// <param name="channel">Model object representing the form submission</param>
        /// <returns>Redirecto the feed list if successful or a return back to the form with errors if it is not</returns>
        [HttpPost]
        public ActionResult AddFeed(RssChannel channel)
        {
            String link = channel.Link;

            RssManager manager = new RssManager(User.Identity.GetUserId());
            RssSubscription existingSubscription = manager.FindSubscription(link);

            if (existingSubscription != null)
            {
                ModelState.AddModelError("", "You are already subscribed to this feed");
                return View();
            }
            else
            {
                try
                {
                    manager.AddSubscription(link);
                }
                catch (RssUpdateException e)
                {
                    ModelState.AddModelError("", "Unable to reach the requested feed");
                    return View();
                }

                return RedirectToAction("ListAllItems");
            }

        }
    }
}