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
        public ActionResult Create([Bind(Include = "Id,AccountId,Name,FeedUrl,RecentPostId,DatePosted,Title,Info")] AspNetFeed aspNetFeed)
        {
            if (ModelState.IsValid)
            {
                //Will search through the list of users to find the one that is currently logged in.
                //Will then create the AspNetUserFeed table based on the Ids of the feedEntry and Feed.
                var userList = db.AspNetUserInfo.ToList();
                foreach (var user in userList)
                {
                    if (user.UserId == User.Identity.GetUserId())
                    {
                        db.AspNetUserFeed.Add(new AspNetUserFeed { AccountId = user.UserId.ToString(), FeedId = aspNetFeed.Id.ToString(), FeedName = aspNetFeed.Name.ToString() });
                    }
                }

                //-------------------------------------------------------
                //Need to Implement how to get the most current post here.
                //-------------------------------------------------------
                aspNetFeed.AccountId = User.Identity.GetUserId();
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
        public ActionResult Edit([Bind(Include = "Id,AccountId,Name,FeedUrl,RecentPostId,DatePosted,Title,Info")] AspNetFeed aspNetFeed)
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

            var userFeedList = db.AspNetUserFeed.ToList();
            foreach (var entry in userFeedList)
            {
                AspNetUserFeed aspNetUserFeed = db.AspNetUserFeed.Find(id);
                db.AspNetUserFeed.Remove(aspNetUserFeed);
            }
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
