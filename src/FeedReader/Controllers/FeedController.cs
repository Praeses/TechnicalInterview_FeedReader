using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FeedReader.Models;
using System.ServiceModel.Syndication;
using System.Xml;

namespace FeedReader.Controllers
{
    [Authorize]
    public class FeedController : Controller
    {
        private FeedDBContext db = new FeedDBContext();

        private int GetNextId()
        {
            if (db.Feeds.Count() == 0)
            {
                return 1;
            }
            return db.Feeds.OrderByDescending(uf => uf.ID).First().ID + 1;
        }

        public Feed AddFeed(string link)
        {
            Feed feed = new Feed();
            feed.Link = link;
            XmlReader reader = XmlReader.Create(feed.Link);
            SyndicationFeed syn_feed = SyndicationFeed.Load(reader);
            reader.Close();
            feed.Title = syn_feed.Title.Text;
            feed.Description = syn_feed.Description.Text;
            feed.ID = GetNextId();
            db.Feeds.Add(feed);
            db.SaveChanges();
            return GetFeed(link);
        }

        public Feed GetFeed(string link)
        {
            return db.Feeds.Where(f => f.Link == link).FirstOrDefault();
        }

        public Feed GetFeed(int id)
        {
            return db.Feeds.Where(f => f.ID == id).FirstOrDefault();
        }

        public IList<Feed> GetFeeds(int[] feed_ids)
        {
            return db.Feeds.Where(f => feed_ids.Contains(f.ID)).OrderBy(f => f.Title).ToList();
        }

        public bool HasFeed(string link)
        {
            if (db.Feeds.Where(f => f.Link == link).Count() > 0)
            {
                return true;
            }
            return false;
        }


        /*
        // GET: /Feed/
        public ActionResult Index()
        {
            return View(db.Feeds.ToList());
        }

        // GET: /Feed/Details/5
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
            return View(feed);
        }

        // GET: /Feed/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Feed/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,Title,Link,Description")] Feed feed)
        {
            if (ModelState.IsValid)
            {
                db.Feeds.Add(feed);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(feed);
        }

        // GET: /Feed/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: /Feed/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,Title,Link,Description")] Feed feed)
        {
            if (ModelState.IsValid)
            {
                db.Entry(feed).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(feed);
        }

        // GET: /Feed/Delete/5
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

        // POST: /Feed/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Feed feed = db.Feeds.Find(id);
            db.Feeds.Remove(feed);
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
