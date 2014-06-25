using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FeedReader.Models;

namespace FeedReader.Controllers
{
    [Authorize]
    public class RssFeedController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //Displays the RssFeeds tied to a given user (GET: /RssFeed/) 
        public ActionResult Index()
        {
            var rssfeeds = db.RssFeeds.Include(r => r.User);
            return View(rssfeeds.ToList());
        }

        //Displays the details of an RssFeed tied to a given user (GET: /RssFeed/)  (GET: /RssFeed/Details/5)
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RssFeed rssfeed = db.RssFeeds.Find(id);
            if (rssfeed == null)
            {
                return HttpNotFound();
            }
            return View(rssfeed);
        }

        // Create a new RssFeed on a given user (GET: /RssFeed/Create)
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email");
            return View();
        }

        // POST: /RssFeed/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="UserId,Title,Description,Link")] RssFeed rssfeed)
        {
            if (ModelState.IsValid)
            {
                db.RssFeeds.Add(rssfeed);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.Users, "Id", "Email", rssfeed.UserId);
            return View(rssfeed);
        }

        // GET: /RssFeed/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RssFeed rssfeed = db.RssFeeds.Find(id);
            if (rssfeed == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email", rssfeed.UserId);
            return View(rssfeed);
        }

        // POST: /RssFeed/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="UserId,Title,Description,Link")] RssFeed rssfeed)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rssfeed).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email", rssfeed.UserId);
            return View(rssfeed);
        }

        // GET: /RssFeed/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RssFeed rssfeed = db.RssFeeds.Find(id);
            if (rssfeed == null)
            {
                return HttpNotFound();
            }
            return View(rssfeed);
        }

        // POST: /RssFeed/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            RssFeed rssfeed = db.RssFeeds.Find(id);
            db.RssFeeds.Remove(rssfeed);
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
    }
}
