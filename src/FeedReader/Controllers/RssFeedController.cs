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
    public class RssFeedController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        //GET: /RssFeed/ShowFeeds
        public ActionResult ShowFeeds()
        {            
            Dictionary<string, List<RssArticle>> articleMap = RssFeedReader.ReadSubscribedFeeds(User.Identity.GetUserId());

            return View(articleMap);
        }

        // GET: /RssFeed/
        public ActionResult Index()
        {
            string UserId = User.Identity.GetUserId();
            var rssfeeds = RssFeedReader.LoadSubscribedFeeds(UserId);
            return View(rssfeeds.ToList());
        }

        // GET: /RssFeed/Details/5
        public ActionResult Details(string UserId, string Title)
        {
            if (UserId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RssFeed rssfeed = db.RssFeeds.Find(UserId,Title);
            if (rssfeed == null)
            {
                return HttpNotFound();
            }
            return View(rssfeed);
        }

        // GET: /RssFeed/Create
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
        public ActionResult Create([Bind(Include="Title,Link")] RssFeed rssfeed)
        {
            rssfeed.UserId = User.Identity.GetUserId();

            if (!RssFeedReader.validateRssLink(rssfeed.Link))
            {
                rssfeed.Link = "An invalid RSS url was used. Please edit the URL and input a valid URL.";
            }

            if (ModelState.IsValid && rssfeed.Title != null)
            {
                db.RssFeeds.Add(rssfeed);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View("Error");
            }

            ViewBag.UserId = new SelectList(db.Users, "Id", "Email", rssfeed.UserId);
            return View(rssfeed);
        }

        // GET: /RssFeed/Edit/5
        public ActionResult Edit(string UserId, string Title)
        {
            if (UserId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            RssFeed rssfeed = db.RssFeeds.Find(UserId,Title);
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
        public ActionResult Edit([Bind(Include="UserId,Title,Link")] RssFeed rssfeed)
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
        public ActionResult Delete(string UserId, string Title)
        {
            if (UserId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RssFeed rssfeed = db.RssFeeds.Find(UserId, Title);
            if (rssfeed == null)
            {
                return HttpNotFound();
            }
            return View(rssfeed);
        }

        // POST: /RssFeed/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string UserId, string Title)
        {
            RssFeed rssfeed = db.RssFeeds.Find(UserId,Title);
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
