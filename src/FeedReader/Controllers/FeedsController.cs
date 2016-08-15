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
using HigLabo.Net.Rss;
using FeedReader.Services;
using System.IO;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

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

        public string CurrentUserId {
            get
            {
                return User.Identity.GetUserId();
            }
        }

        // GET: Feeds
        public ActionResult Index()
        {
            return View(db.Feeds.ToList());
        }

        // GET: Feeds/Details/5
        public ActionResult Details(int? id)
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
            var rssFeed = rssClient.GetRssFeed(feed.URL);
            return View(rssFeed);
        }

        // GET: Feeds/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Feeds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FeedId,URL")] Feed feed)
        {
            if (ModelState.IsValid && IsValidRssEndpoint(feed.URL))
            {
                feed.UserId = CurrentUserId;
                var rssFeed = rssClient.GetRssFeed(feed.URL);
                feed.Image = rssFeed.ImageUrl;
                feed.Title = rssFeed.Title;
                db.Feeds.Add(feed);
                db.SaveChanges();
            }

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
            catch(Exception ex)
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
