using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FeedReader.Models;
using Microsoft.AspNet.Identity;

namespace FeedReader.Controllers
{
    [Authorize]
    public class UserFeedController : Controller
    {
        private FeedDBContext db = new FeedDBContext();

        private int GetNextId()
        {
            if (db.UserFeeds.Count() == 0)
            {
                return 1;
            }
            return db.UserFeeds.OrderByDescending(uf => uf.ID).First().ID + 1;
        }

        public IList<UserFeed> AddUserFeed(string link, string user_id)
        {
            FeedController fc = new FeedController();
            Feed feed;

            //Don't duplicate
            if(CheckUserFeed(link, user_id) == true)
            {
                return GetUserFeeds(user_id);
            }

            //Loads feed if needed
            if (fc.HasFeed(link) == false)
            {
                feed = fc.AddFeed(link);
                FeedItemController fic = new FeedItemController();
                fic.AddFeedItems(feed);
            }
            else 
            {
                feed = fc.GetFeed(link);
            }

            UserFeed userfeed = new UserFeed();
            userfeed.FeedId = feed.ID;
            userfeed.UserId = user_id;
            userfeed.ID = GetNextId();
            db.UserFeeds.Add(userfeed);
            db.SaveChanges();

            return GetUserFeeds(user_id);
        }

        public IList<UserFeed> GetUserFeeds(string user_id)
        {
            return db.UserFeeds.Where(uf => uf.UserId.CompareTo(user_id) == 0).ToList();
        }

        public IList<Feed> GetFeedsByUser(string user_id)
        {
            FeedController fc = new FeedController();
            return fc.GetFeeds(GetFeedIds(user_id));
        }


        public int[] GetFeedIds(string user_id)
        {
            int[] ids = new int[GetUserFeedCount(user_id)];
            int counter = 0;
            foreach (UserFeed userfeed in GetUserFeeds(user_id))
            {
                ids[counter] = userfeed.FeedId;
                counter++;
            }

            return ids;
        }

        public int GetUserFeedCount(string user_id)
        {
            return GetUserFeeds(user_id).Count();
        }

        public bool HasUserFeed(string user_id)
        {
            if(GetUserFeedCount(user_id) > 0)
            {
                return true;
            }
            return false;
        }

        public bool CheckUserFeed(string link, string user_id)
        {
            if(db.UserFeeds.Where(uf => uf.Feed.Link == link && uf.UserId == user_id).Count() > 0)
            {
                return true;
            }
            return false;
        }

        [ChildActionOnly]
        public ActionResult LoadUserFeedWindow(string user_id)
        {
            return this.PartialView("_UserFeedWindow", GetFeedsByUser(user_id));
        }

        /*
        // GET: /UserFeed/
        public ActionResult Index()
        {
            var userfeeds = db.UserFeeds.Include(u => u.Feed);
            return View(userfeeds.ToList());
        }

        // GET: /UserFeed/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserFeed userfeed = db.UserFeeds.Find(id);
            if (userfeed == null)
            {
                return HttpNotFound();
            }
            return View(userfeed);
        }

        // GET: /UserFeed/Create
        public ActionResult Create()
        {
            ViewBag.FeedId = new SelectList(db.Feeds, "ID", "Title");
            return View();
        }

        // POST: /UserFeed/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,UserId,FeedId")] UserFeed userfeed)
        {
            if (ModelState.IsValid)
            {
                db.UserFeeds.Add(userfeed);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FeedId = new SelectList(db.Feeds, "ID", "Title", userfeed.FeedId);
            return View(userfeed);
        }

        // GET: /UserFeed/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserFeed userfeed = db.UserFeeds.Find(id);
            if (userfeed == null)
            {
                return HttpNotFound();
            }
            ViewBag.FeedId = new SelectList(db.Feeds, "ID", "Title", userfeed.FeedId);
            return View(userfeed);
        }

        // POST: /UserFeed/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,UserId,FeedId")] UserFeed userfeed)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userfeed).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FeedId = new SelectList(db.Feeds, "ID", "Title", userfeed.FeedId);
            return View(userfeed);
        }

        // GET: /UserFeed/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserFeed userfeed = db.UserFeeds.Find(id);
            if (userfeed == null)
            {
                return HttpNotFound();
            }
            return View(userfeed);
        }

        // POST: /UserFeed/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserFeed userfeed = db.UserFeeds.Find(id);
            db.UserFeeds.Remove(userfeed);
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
