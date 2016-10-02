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
            return RedirectToAction("ListFeeds");
        }

        public ActionResult ListFeeds(bool refreshFeed = true)
        {
            ViewData["refreshFeed"] = refreshFeed;
            return View();
        }

        [HttpPost]
        public JsonResult UpdateFeeds()
        {
            RssManager manager = new RssManager(User.Identity.GetUserId());
            manager.RequestChannelUpdate();

            return Json("success");
        }

        public ActionResult RemoveSubscription(int rssChannelId)
        {
            RssManager manager = new RssManager(User.Identity.GetUserId());
            manager.RemoveSubscription(rssChannelId);
            return RedirectToAction("ListFeeds");
        }

        [HttpPost]
        public JsonResult UpdateFeedItem(int rssItemId, bool hide = false)
        {
            string userId = User.Identity.GetUserId();
            RssManager manager = new RssManager(userId);
            manager.UpdateRssItem(rssItemId, hide,true);
          
            return Json("success");
        }

        [HttpPost]
        public virtual JsonResult FeedJson(DTableRequest dTableRequest, bool refresh = false)
        {
            RssManager manager = new RssManager(User.Identity.GetUserId());   
            string userId = User.Identity.GetUserId();

            dTableRequest.search.value = Request.Params["search[value]"]; //TO-DO: find out why second level objects are not parsed by mvc

            DTableResponse<Object> dTableResponse = manager.RetrieveDataTableRssItems(dTableRequest);

            return Json(dTableResponse, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewFeed(string feedUrl)
        {
            RssContext context = new RssContext();
            RssChannel channel = context.RssChannels.Include("Items").Where(itemId => itemId.FeedUrl == feedUrl).FirstOrDefault();
            channel.Items = new List<RssItem>(channel.Items.OrderByDescending(a => a.PubDate));

            return View(channel);
        }

        public ActionResult ViewFeedItem(int feedItemId)
        {
            RssContext context = new RssContext();
            RssItem rssItem = context.RssItems.Include(a=>a.Channel).FirstOrDefault(a=> a.RssItemId == feedItemId);

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

            RssManager manager = new RssManager(User.Identity.GetUserId());
            RssSubscription existingSubscription = manager.FindSubscription(link);

            if (existingSubscription != null)
            {
                ModelState.AddModelError("", "You are already subscribed to this feed");
                return View();
            }
            else
            {
                manager.AddSubscription(link);
                return RedirectToAction("ListFeeds");
            }

        }
    }
}