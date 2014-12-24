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
    public class UserFeedController : Controller
    {
        private UserContext db = new UserContext();

        // GET: UserFeed
        public ActionResult Index()
        {
            return View(db.AspNetUserFeed.ToList());
        }

        // GET: UserFeed/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUserFeed aspNetUserFeed = db.AspNetUserFeed.Find(id);
            if (aspNetUserFeed == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUserFeed);
        }

        // GET: UserFeed/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserFeed/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,FeedId,FeedName,Title,Info")] AspNetUserFeed aspNetUserFeed)
        {
            if (ModelState.IsValid)
            {
                db.AspNetUserFeed.Add(aspNetUserFeed);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aspNetUserFeed);
        }

        // GET: UserFeed/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUserFeed aspNetUserFeed = db.AspNetUserFeed.Find(id);
            if (aspNetUserFeed == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUserFeed);
        }

        // POST: UserFeed/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,FeedId,FeedName,Title,Info")] AspNetUserFeed aspNetUserFeed)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetUserFeed).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aspNetUserFeed);
        }

        // GET: UserFeed/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUserFeed aspNetUserFeed = db.AspNetUserFeed.Find(id);
            if (aspNetUserFeed == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUserFeed);
        }

        // POST: UserFeed/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AspNetUserFeed aspNetUserFeed = db.AspNetUserFeed.Find(id);
            db.AspNetUserFeed.Remove(aspNetUserFeed);
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
