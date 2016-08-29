using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using FeedReader.Models;
using Microsoft.AspNet.Identity;
using FeedReader.Services;
using System.IO;

namespace FeedReader.Controllers
{
    [Authorize]
    public class FeedsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CachingRssClient rssClient = new CachingRssClient();
        private const string RssFeedSearchUrl = "https://feedly.com/v3/search/auto-complete";
        private const int SearchRequestTimeout = 5000;
        private const int DefaultSearchSitesToRetrieve = 5;
        private const int MaxSitesToRetrieve = 20;

        public string CurrentUserId
        {
            get
            {
                return User.Identity.GetUserId();
            }
        }

        // GET: Feeds
        public ActionResult Index()
        {
            return View(GetFeeds().ToList());
        }

        private IEnumerable<Feed> GetFeeds()
        {
            return db.Feeds.Where(x => x.UserId == CurrentUserId);
        }

        // GET: Feeds/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feed feed = GetFeeds().FirstOrDefault(x => x.FeedId == id);
            if (feed == null)
            {
                return HttpNotFound();
            }
            return View(feed);
        }

        // POST: Feeds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FeedId,URL")] Feed feed)
        {
            if (!ModelState.IsValid || !IsValidRssEndpoint(feed.URL))
            {
                TempData["Error"] = "The RSS Feed you are trying to add (" + feed.URL + ") is not valid.";
                return RedirectToAction("Index");
            }
            feed.UserId = CurrentUserId;

            // We want to see if we already have this feed. If so, we know it's good to add, just duplicate it for this user and return.
            var existingFeed = db.Feeds
                .Where(x => x.URL.Equals(feed.URL, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            if (existingFeed != null)
            {
                feed.Image = existingFeed.Image;
                feed.Title = existingFeed.Title;
                db.Feeds.Add(feed);
                return RedirectToAction("Index");
            }

            // We didn't have this feed already so let's make sure we can get it and cache the image and title.
            //FIXME: Right now this doesn't ever update feed items after initially retrieving it. 
            // Maybe there needs to be a background job or service that handles this.
            var rssFeed = rssClient.GetRssFeed(feed.URL);
            feed.Image = rssFeed.ImageUrl;
            feed.Title = rssFeed.Title;
            db.Feeds.Add(feed);

            var feedItems = rssFeed.Items.Select(item => new FeedItem
            {
                Feed = feed,
                Description = item.Description,
                Image = item.ImageUrl,
                PublishedDate = item.PublishedDate,
                Title = item.Title,
                URL = item.Url
            });
            db.FeedItems.AddRange(feedItems);

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Feeds/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feed feed = db.Feeds.Find(id);
            if (feed == null)
            {
                return HttpNotFound();
            }
            return View(feed);
        }

        // POST: Feeds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Feed feed = db.Feeds.Find(id);
            db.Feeds.Remove(feed);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Search(string query, int? sites)
        {
            if (String.IsNullOrWhiteSpace(query))
            {
                return new HttpStatusCodeResult(422, "The query param must be present and not empty.");
            }

            var sitesToRequest = (sites.HasValue && (sites <= 0)) ? DefaultSearchSitesToRetrieve : Math.Min(sites.Value, MaxSitesToRetrieve);
            var searchUrl = String.Format("{0}?query={1}&sites={2}", RssFeedSearchUrl, query, sitesToRequest);

            try
            {
                var request = WebRequest.CreateHttp(searchUrl);
                request.Timeout = SearchRequestTimeout;
                var response = request.GetResponse();

                string content;
                using (var dataStream = response.GetResponseStream())
                {
                    var reader = new StreamReader(dataStream);
                    content = reader.ReadToEnd();
                }

                return Content(content, response.ContentType);
            }
            catch (WebException ex)
            {
                return new HttpStatusCodeResult(502, String.Format("The search service is currently unavailable. It returned {0} - {1}", ex.Status, ex.Message));
            }
        }

        private bool IsValidRssEndpoint(string url)
        {
            try
            {
                rssClient.GetRssFeed(url);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
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
