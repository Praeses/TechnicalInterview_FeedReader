using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FeedReader.Models;
using Microsoft.AspNet.Identity;

namespace FeedReader.Controllers
{
    [Authorize]
    public class FeedItemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private const int DefaultPageSize = 20;

        // GET: FeedItems
        public ActionResult Index(string query, int? page)
        {
            var feedItems = GetFeedItems();
            if (!String.IsNullOrWhiteSpace(query))
            {
                feedItems = feedItems.Where(x => x.Title.Contains(query));
            }
            ViewBag.Query = query;
            return View(PaginateFeedItems(feedItems, page ?? 1));
        }

        private IEnumerable<FeedItem> GetFeedItems()
        {
            var userId = User.Identity.GetUserId();
            return db.FeedItems.Include(from => from.Feed)
                .Where(x => x.Feed.UserId == userId)
                .OrderBy(x => x.PublishedDate);
        }

        private IEnumerable<FeedItem> PaginateFeedItems(IEnumerable<FeedItem> feedItems, int page)
        {
            ViewBag.TotalPages = feedItems.Count() / DefaultPageSize;
            ViewBag.CurrentPage = page;
            return feedItems.Skip((page - 1) * DefaultPageSize)
                .Take(DefaultPageSize);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
