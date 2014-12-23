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
    public class FeedController : Controller
    {
        private UserContext db = new UserContext();

        // GET: Feed
        public ActionResult Index()
        {
            return View(db.AspNetFeed.ToList());
        }

        // GET: Feed/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetFeed aspNetFeed = db.AspNetFeed.Find(id);
            if (aspNetFeed == null)
            {
                return HttpNotFound();
            }
            return View(aspNetFeed);
        }

        // GET: Feed/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Feed/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,FeedUrl,Date")] AspNetFeed aspNetFeed)
        {
            if (ModelState.IsValid)
            {
                db.AspNetFeed.Add(aspNetFeed);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aspNetFeed);
        }

        // GET: Feed/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetFeed aspNetFeed = db.AspNetFeed.Find(id);
            if (aspNetFeed == null)
            {
                return HttpNotFound();
            }
            return View(aspNetFeed);
        }

        // POST: Feed/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,FeedUrl,Date")] AspNetFeed aspNetFeed)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetFeed).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aspNetFeed);
        }

        // GET: Feed/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetFeed aspNetFeed = db.AspNetFeed.Find(id);
            if (aspNetFeed == null)
            {
                return HttpNotFound();
            }
            return View(aspNetFeed);
        }

        // POST: Feed/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AspNetFeed aspNetFeed = db.AspNetFeed.Find(id);
            db.AspNetFeed.Remove(aspNetFeed);
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
