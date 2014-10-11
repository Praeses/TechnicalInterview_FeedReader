using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FeedReader.Models;
using System.Xml;
using System.ServiceModel.Syndication;

namespace FeedReader.Controllers
{
    [Authorize]
    public class FeedItemController : Controller
    {
        private FeedDBContext db = new FeedDBContext();

        private int GetNextId()
        {
            if (db.FeedItems.Count() == 0)
            {
                return 1;
            }
            return db.FeedItems.OrderByDescending(uf => uf.ID).First().ID + 1;
        }

        public IList<FeedItem> AddFeedItems(Feed feed)
        {
            XmlReader reader = XmlReader.Create(feed.Link);
            SyndicationFeed syn_feed = SyndicationFeed.Load(reader);
            reader.Close();
            foreach (SyndicationItem item in syn_feed.Items)
            {
                FeedItem feeditem = new FeedItem();
                feeditem.Title = item.Title.Text;
                feeditem.Link = item.Id.ToString();
                //Trim any garbage off the front
                feeditem.Link = feeditem.Link.Substring(feeditem.Link.IndexOf("http"));
                feeditem.Description = item.Summary.Text;
                feeditem.FeedId = feed.ID;
                feeditem.PublishDate = item.PublishDate.DateTime;
                feeditem.ID = GetNextId();
                db.FeedItems.Add(feeditem);
                db.SaveChanges();
            }

            return GetFeedItems(feed.ID);
        }

        public IList<FeedItem> GetFeedItems(int feed_id, string user_id)
        {
            if (feed_id == 0)
            {
                return GetAllUserFeedItems(user_id);
            }
            FeedController fc = new FeedController();
            return GetFeedItems(feed_id);
        }

        public IList<FeedItem> GetFeedItems(int feed_id)
        {
            return db.FeedItems.Where(fi => fi.FeedId == feed_id).OrderByDescending(fi => fi.PublishDate).ToList();
        }

        //Used in search
        public IList<FeedItem> GetFeedItems(string needle, string user_id) 
        {
            UserFeedController ufc = new UserFeedController();
            int[] feed_ids = ufc.GetFeedIds(user_id);
            return db.FeedItems.Where(fi => fi.Title.Contains(needle) && feed_ids.Contains(fi.FeedId)).OrderByDescending(fi => fi.PublishDate).ToList();
        }

        //Used in search
        public IList<FeedItem> GetFeedItems(string needle, int feed_id) 
        {
            return db.FeedItems.Where(fi => fi.Title.Contains(needle) && fi.FeedId == feed_id).OrderByDescending(fi => fi.PublishDate).ToList();
        }

        public IList<FeedItem> GetAllUserFeedItems(string user_id)
        {
            UserFeedController ufc = new UserFeedController();
            int[] feed_ids = ufc.GetFeedIds(user_id);
            return db.FeedItems.Where(fi => feed_ids.Contains(fi.FeedId)).OrderByDescending(fi => fi.PublishDate).ToList();
        }

        public bool HasFeedItem(int feed_id, string user_id)
        {
            FeedController fc = new FeedController();
            if (feed_id == 0)
            {
                if (GetAllUserFeedItems(user_id).Count() > 0)
                {
                    return true;
                }
                return false;
            }
            return HasFeedItem(fc.GetFeed(feed_id));
        }

        public bool HasFeedItem(Feed feed)
        {
            if (db.FeedItems.Where(fi => fi.FeedId == feed.ID).Count() > 0)
            {
                return true;
            }
            return false;
        }

        public IList<FeedItem> SearchUserFeeds(string needle, string user_id)
        {
            UserFeedController ufc = new UserFeedController();
            int[] feed_ids = ufc.GetFeedIds(user_id);
            return db.FeedItems.Where(fi => feed_ids.Contains(fi.FeedId) && fi.Title.ToLower().Contains(needle.ToLower())).ToList();
        }

        public IList<FeedItem> SearchFeed(string needle, int feed_id)
        {
            return db.FeedItems.Where(fi => fi.FeedId == feed_id && fi.Title.ToLower().Contains(needle.ToLower())).ToList();
        }

        [ChildActionOnly]
        public ActionResult LoadFeedItemWindow(int feed_id, string user_id)
        {
            return this.PartialView("_FeedItemWindow", GetFeedItems(feed_id, user_id));
        }

        [ChildActionOnly]
        public ActionResult SearchFeedItemWindow(string needle, int feed_id, string user_id)
        {
            if (feed_id == 0)
            {
                return this.PartialView("_FeedItemWindow", SearchUserFeeds(needle, user_id));
            }
            return this.PartialView("_FeedItemWindow", SearchFeed(needle, feed_id));
        }
        /*
        // GET: /FeedItem/
        public ActionResult Index()
        {
            var feeditems = db.FeedItems.Include(f => f.Feed);
            return View(feeditems.ToList());
        }

        // GET: /FeedItem/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeedItem feeditem = db.FeedItems.Find(id);
            if (feeditem == null)
            {
                return HttpNotFound();
            }
            return View(feeditem);
        }

        // GET: /FeedItem/Create
        public ActionResult Create()
        {
            ViewBag.FeedId = new SelectList(db.Feeds, "ID", "Title");
            return View();
        }

        // POST: /FeedItem/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,Title,Link,Description,PublishDate,FeedId")] FeedItem feeditem)
        {
            if (ModelState.IsValid)
            {
                db.FeedItems.Add(feeditem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FeedId = new SelectList(db.Feeds, "ID", "Title", feeditem.FeedId);
            return View(feeditem);
        }

        // GET: /FeedItem/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeedItem feeditem = db.FeedItems.Find(id);
            if (feeditem == null)
            {
                return HttpNotFound();
            }
            ViewBag.FeedId = new SelectList(db.Feeds, "ID", "Title", feeditem.FeedId);
            return View(feeditem);
        }

        // POST: /FeedItem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,Title,Link,Description,PublishDate,FeedId")] FeedItem feeditem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(feeditem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FeedId = new SelectList(db.Feeds, "ID", "Title", feeditem.FeedId);
            return View(feeditem);
        }

        // GET: /FeedItem/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeedItem feeditem = db.FeedItems.Find(id);
            if (feeditem == null)
            {
                return HttpNotFound();
            }
            return View(feeditem);
        }

        // POST: /FeedItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FeedItem feeditem = db.FeedItems.Find(id);
            db.FeedItems.Remove(feeditem);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        */
    }
}
