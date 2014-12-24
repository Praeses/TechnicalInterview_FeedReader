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
    public class PostsController : Controller
    {
        private UserContext db = new UserContext();

        // GET: AspNetPosts
        public ActionResult Index()
        {
            return View(db.AspNetPosts.ToList());
        }

        // GET: AspNetPosts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetPost aspNetPost = db.AspNetPosts.Find(id);
            if (aspNetPost == null)
            {
                return HttpNotFound();
            }
            return View(aspNetPost);
        }

        // GET: AspNetPosts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AspNetPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FeedId,Author,Title,Body,Date")] AspNetPost aspNetPost)
        {
            if (ModelState.IsValid)
            {
                //Will search through the list of users to find the one that is currently logged in.
                //Will then create the AspNetUserFeed table based on the Ids of the feedEntry and Feed.
                var feedList = db.AspNetFeed.ToList();
                foreach (var feedEntry in feedList)
                {
                    if (feedEntry.Id.ToString().Trim() == aspNetPost.FeedId.Trim())
                    {
                        //Removes the AspNetFeed database entry
                        //Updates the data to aspNetFeed
                        //Adds aspNetFeed to the database with the updated Info.
                        AspNetFeed aspNetFeed = db.AspNetFeed.Find(feedEntry.Id);
                        db.AspNetFeed.Remove(aspNetFeed);
                        aspNetFeed.Id = aspNetFeed.Id;
                        aspNetFeed.RecentPostId = aspNetPost.Id.ToString();
                        aspNetFeed.DatePosted = aspNetPost.Date;
                        aspNetFeed.Title = aspNetPost.Title;
                        aspNetFeed.Info = aspNetPost.Body;
                        
                        var userFeedList = db.AspNetUserFeed.ToList();
                        foreach (var entry in userFeedList)
                        {
                            if (User.Identity.GetUserId() == entry.AccountId && aspNetFeed.Id.ToString() == entry.FeedId)
                            {
                                //Removes the AspNetUserFeed that matches from the database
                                //Updates the data for tempEntry
                                //Adds tempEntry to the database with the updated Info.
                                var tempEntry = entry;
                                db.AspNetUserFeed.Remove(tempEntry);
                                tempEntry.Id = tempEntry.Id;
                                tempEntry.Title = aspNetPost.Title;
                                tempEntry.Info = aspNetPost.Body;
                                db.AspNetUserFeed.Add(tempEntry);
                            }
                        }
                    }
                }

                db.AspNetPosts.Add(aspNetPost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aspNetPost);
        }

        // GET: AspNetPosts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetPost aspNetPost = db.AspNetPosts.Find(id);
            if (aspNetPost == null)
            {
                return HttpNotFound();
            }
            return View(aspNetPost);
        }

        // POST: AspNetPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FeedId,Author,Title,Body,Date")] AspNetPost aspNetPost)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetPost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aspNetPost);
        }

        // GET: AspNetPosts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetPost aspNetPost = db.AspNetPosts.Find(id);
            if (aspNetPost == null)
            {
                return HttpNotFound();
            }
            return View(aspNetPost);
        }

        // POST: AspNetPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AspNetPost aspNetPost = db.AspNetPosts.Find(id);

            //------------------------------------------------------------------
            //Code to remove a single post
            //Needs to check if it is most current post, if not, simply remove
            //-----------------------------------------------------------------

            db.AspNetPosts.Remove(aspNetPost);
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
