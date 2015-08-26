using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using FeedReader.Models;
using Microsoft.AspNet.Identity.Owin;

namespace FeedReader.Controllers
{
    public class NewsFeedItemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: NewsFeedItems
        public async Task<ActionResult> Index()
        {
            var newsFeedItems = db.NewsFeedItems.Include(n => n.NewsFeed);

            var returnItems = await newsFeedItems.ToListAsync();

            returnItems.Sort();
            returnItems.Reverse();

            return View(returnItems);
        }

        // POST: NewsFeedItems
        [HttpPost]
        public async Task<ActionResult> Index(string search)
        {
            if (search == null ||
                search == String.Empty)
            {
                var newsFeedItems = db.NewsFeedItems.Include(n => n.NewsFeed);
                
                var returnItems = await newsFeedItems.ToListAsync();

                returnItems.Sort();
                returnItems.Reverse();

                return View(returnItems);
            }
            else
            {
                var newsFeedItems = await db.NewsFeedItems.Include(n => n.NewsFeed).ToListAsync();

                var matchingItems = from n in newsFeedItems
                                    where Regex.IsMatch(n.Title, "\\b" + search + "\\b", RegexOptions.IgnoreCase) || 
                                          Regex.IsMatch(n.Description, "\\b" + search + "\\b", RegexOptions.IgnoreCase)
                                    select n;
                
                if (matchingItems == null ||
                    matchingItems.Count() == 0)
                {
                    return HttpNotFound();
                }

                var returnItems = matchingItems.ToList();

                returnItems.Sort();
                returnItems.Reverse();

                return View(returnItems);
            }
        }

        [Authorize()]
        public ActionResult My()
        {
            string userID = User.Identity.GetUserId();

            ApplicationUser user = db.Users.Where(u => u.Id == userID).SingleOrDefault();

            List<NewsFeedItem> itemsToDisplay = new List<NewsFeedItem>();

            foreach (NewsFeed newsFeed in user.NewsFeeds)
            {
                itemsToDisplay.AddRange(newsFeed.Items);
            }

            var returnItems = itemsToDisplay.ToList();

            returnItems.Sort();
            returnItems.Reverse();

            return View(returnItems);                         
        }

        [HttpPost]
        public ActionResult My(string search)
        {
            string userID = User.Identity.GetUserId();

            ApplicationUser user = db.Users.Where(u => u.Id == userID).SingleOrDefault();

            if (search == null ||
                search == String.Empty)
            {                
                List<NewsFeedItem> itemsToDisplay = new List<NewsFeedItem>();

                foreach (NewsFeed newsFeed in user.NewsFeeds)
                {
                    itemsToDisplay.AddRange(newsFeed.Items);
                }

                var returnItems = itemsToDisplay.ToList();

                returnItems.Sort();
                returnItems.Reverse();

                return View(returnItems);
            }
            else
            {
                List<NewsFeedItem> allItems = new List<NewsFeedItem>();

                foreach (NewsFeed newsFeed in user.NewsFeeds)
                {
                    allItems.AddRange(newsFeed.Items);
                }
                             
                var matchingItems = from n in allItems
                                    where Regex.IsMatch(n.Title, "\\b" + search + "\\b", RegexOptions.IgnoreCase) ||  
                                          Regex.IsMatch(n.Description, "\\b" + search + "\\b", RegexOptions.IgnoreCase)
                                    select n;

                if (matchingItems == null ||
                    matchingItems.Count() == 0)
                {
                    return HttpNotFound();
                }

                var returnItems = matchingItems.ToList();

                returnItems.Sort();
                returnItems.Reverse();

                return View(returnItems);
            }
        }
        
        // GET: NewsFeedItems/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewsFeedItem newsFeedItem = await db.NewsFeedItems.FindAsync(id);
            if (newsFeedItem == null)
            {
                return HttpNotFound();
            }
            return View(newsFeedItem);
        }

        // GET: NewsFeedItems/Create
        [Authorize()]
        public ActionResult Create()
        {
            ViewBag.NewsFeedID = new SelectList(db.NewsFeed, "NewsFeedID", "Category");

            var userID = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Where(u => u.Id == userID).SingleOrDefault();

            NewsFeedItem item = new NewsFeedItem();

            item.CreatedBy = user.Alias;
            item.DateAdded = DateTime.Now;

            return View(item);
        }

        // POST: NewsFeedItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize()]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "NewsFeedItemID,NewsFeedID,Title,Description,DateAdded,CreatedBy")] NewsFeedItem newsFeedItem)
        {
            if (ModelState.IsValid)
            {
                db.NewsFeedItems.Add(newsFeedItem);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.NewsFeedID = new SelectList(db.NewsFeed, "NewsFeedID", "Category", newsFeedItem.NewsFeedID);
            return View(newsFeedItem);
        }

        // GET: NewsFeedItems/Edit/5
        [Authorize()]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewsFeedItem newsFeedItem = await db.NewsFeedItems.FindAsync(id);
            if (newsFeedItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.NewsFeedID = new SelectList(db.NewsFeed, "NewsFeedID", "Category", newsFeedItem.NewsFeedID);
            return View(newsFeedItem);
        }

        // POST: NewsFeedItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize()]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "NewsFeedItemID,NewsFeedID,Title,Description,DateAdded,CreatedBy")] NewsFeedItem newsFeedItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(newsFeedItem).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.NewsFeedID = new SelectList(db.NewsFeed, "NewsFeedID", "Category", newsFeedItem.NewsFeedID);
            return View(newsFeedItem);
        }

        // GET: NewsFeedItems/Delete/5
        [Authorize()]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewsFeedItem newsFeedItem = await db.NewsFeedItems.FindAsync(id);
            if (newsFeedItem == null)
            {
                return HttpNotFound();
            }
            return View(newsFeedItem);
        }

        // POST: NewsFeedItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize()]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            NewsFeedItem newsFeedItem = await db.NewsFeedItems.FindAsync(id);
            db.NewsFeedItems.Remove(newsFeedItem);
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
