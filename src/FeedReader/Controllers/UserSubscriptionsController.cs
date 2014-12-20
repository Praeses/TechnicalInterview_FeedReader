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
    public class UserSubscriptionsController : Controller
    {
        private UserSubscriptionDBContext db = new UserSubscriptionDBContext();

        // GET: UserSubscriptions
        public ActionResult Index(string searchString)
        {
            string username = User.Identity.GetUserName();//FIXME: Not sure if this is safe?
            var subscriptions = from s in db.UserSubscriptions
                                select s;
            subscriptions = subscriptions.Where(n => n.userName.Equals(username));
            if(!String.IsNullOrEmpty(searchString))
            {
                subscriptions = subscriptions.Where(n => n.rssFeedURL.Contains(searchString));
            }

            return View(subscriptions);
        }

        // GET: UserSubscriptions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserSubscription userSubscription = db.UserSubscriptions.Find(id);
            if (userSubscription == null)
            {
                return HttpNotFound();
            }
            return View(userSubscription);
        }

        // GET: UserSubscriptions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserSubscriptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,userName,rssFeedURL, rssFeedName")] UserSubscription userSubscription)
        {
            string username = User.Identity.GetUserName();//FIXME: Not sure if this is safe
            if (username != null && username != "")
            {
                if(userSubscription != null)//Check for valid entry
                {
                    if (ModelState.IsValid)
                    {
                        userSubscription.userName = username;
                        db.UserSubscriptions.Add(userSubscription);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
            }
            return View(userSubscription);
        }

        // GET: UserSubscriptions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserSubscription userSubscription = db.UserSubscriptions.Find(id);
            if (userSubscription == null)
            {
                return HttpNotFound();
            }
            return View(userSubscription);
        }

        // POST: UserSubscriptions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,userName,rssFeedURL")] UserSubscription userSubscription)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userSubscription).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userSubscription);
        }

        // GET: UserSubscriptions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserSubscription userSubscription = db.UserSubscriptions.Find(id);
            if (userSubscription == null)
            {
                return HttpNotFound();
            }
            return View(userSubscription);
        }

        // POST: UserSubscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserSubscription userSubscription = db.UserSubscriptions.Find(id);
            db.UserSubscriptions.Remove(userSubscription);
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
