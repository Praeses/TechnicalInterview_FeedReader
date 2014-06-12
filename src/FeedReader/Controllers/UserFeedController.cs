using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FeedReader.Models;

namespace FeedReader.Controllers
{
    [Authorize]
    public class UserFeedController : Controller
    {
        private UserFeedContext db = new UserFeedContext();

        // GET: /UserFeed/
        public async Task<ActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            string currentUserId = User.Identity.GetUserId();

            var myfeeds = from f in db.UserFeedModel
                          select f;
            myfeeds = myfeeds.Where(f => f.UserID.ToString().Equals(currentUserId));
            myfeeds = myfeeds.OrderBy(f => f.Title);

            return View(await myfeeds.ToListAsync());
        }
        
        public ActionResult ListItems(int? page, string searchFilter)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (searchFilter != null)
            {
                page = 1;
            }
            
            string currentUserId = User.Identity.GetUserId();

            var myfeeds = (from f in db.UserFeedModel
                          where f.UserID == currentUserId
                          select f.Url).ToList();
            
            IEnumerable<Rss> items = new List<Rss>();
            foreach (var url in myfeeds)
            {
                items = items.Concat(RssReader.GetRss(url));
            }

            if (!String.IsNullOrEmpty(searchFilter))
            {
                items = items.Where(rss => rss.Title.ToUpper().Contains(searchFilter.ToUpper()) || 
                    rss.Description.ToUpper().Contains(searchFilter.ToUpper()));
            }
            items = items.OrderByDescending(i => i.PubDate);
            
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(items.ToPagedList(pageNumber, pageSize));
        }
        
        public async Task<ActionResult> List(int? id, string searchFilter)
        {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UserFeedModel userfeedmodel = await db.UserFeedModel.FindAsync(id);
            if (userfeedmodel == null)
            {
                return HttpNotFound();
            }

            var items = from item in RssReader.GetRss(userfeedmodel.Url)
                        select item;
            if (!String.IsNullOrEmpty(searchFilter))
            {
                items = items.Where(item => item.Title.ToUpper().Contains(searchFilter.ToUpper()) || item.Description.ToUpper().Contains(searchFilter.ToUpper()));
            }
            userfeedmodel.FeedList = items;
            
            return View(userfeedmodel);
        }

     

        // GET: /UserFeed/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserFeedModel userfeedmodel = await db.UserFeedModel.FindAsync(id);
            if (userfeedmodel == null)
            {
                return HttpNotFound();
            }
            System.Diagnostics.Debug.WriteLine("title = " + RssReader.GetRssTitle(userfeedmodel.Url));

            return View(userfeedmodel);
        }

        // GET: /UserFeed/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /UserFeed/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="UserFeedID,UserID,Url")] UserFeedModel userfeedmodel)
        {
            if (ModelState.IsValid)
            {
                userfeedmodel.Title = RssReader.GetRssTitle(userfeedmodel.Url);
                userfeedmodel.UserID = User.Identity.GetUserId();
                db.UserFeedModel.Add(userfeedmodel);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(userfeedmodel);
        }

        // GET: /UserFeed/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserFeedModel userfeedmodel = await db.UserFeedModel.FindAsync(id);
            if (userfeedmodel == null)
            {
                return HttpNotFound();
            }
            if (userfeedmodel.Title == null)
            {
                userfeedmodel.Title = RssReader.GetRssTitle(userfeedmodel.Url);
            }
            return View(userfeedmodel);
        }

        // POST: /UserFeed/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="UserFeedID,UserID,Url")] UserFeedModel userfeedmodel)
        {
            if (ModelState.IsValid)
            {
                userfeedmodel.Title = RssReader.GetRssTitle(userfeedmodel.Url);
                db.Entry(userfeedmodel).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(userfeedmodel);
        }

        // GET: /UserFeed/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserFeedModel userfeedmodel = await db.UserFeedModel.FindAsync(id);
            if (userfeedmodel == null)
            {
                return HttpNotFound();
            }
            return View(userfeedmodel);
        }

        // POST: /UserFeed/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            UserFeedModel userfeedmodel = await db.UserFeedModel.FindAsync(id);
            db.UserFeedModel.Remove(userfeedmodel);
            await db.SaveChangesAsync();
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
