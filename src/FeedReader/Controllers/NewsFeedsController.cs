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
using Microsoft.AspNet.Identity.Owin;

namespace FeedReader.Controllers
{
    public class NewsFeedsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: NewsFeeds
        [Authorize(Roles = "Admin")]
        [Route("Fitzgeralds-Flash/All")]
        public async Task<ActionResult> Index()
        {
            return View(await db.NewsFeed.ToListAsync());
        }

        // GET: NewsFeeds/Details/5
        [Authorize(Roles = "Admin")]
        [Route("Fitzgeralds-Flash/{id:int}")]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewsFeed newsFeed = await db.NewsFeed.FindAsync(id);
            if (newsFeed == null)
            {
                return HttpNotFound();
            }
            return View(newsFeed);
        }

        // GET: NewsFeeds/Create
        [Authorize(Roles = "Admin")]
        [Route("Fitzgeralds-Flash/Create")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: NewsFeeds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("Fitzgeralds-Flash/Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "NewsFeedID,Category")] NewsFeed newsFeed)
        {
            if (ModelState.IsValid)
            {
                db.NewsFeed.Add(newsFeed);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ApplicationData.AllNewsFeeds = db.NewsFeed.ToList();

            ApplicationData.AllNewsFeeds.Sort();

            return View(newsFeed);
        }

        // GET: NewsFeeds/Edit/5
        [Authorize(Roles = "Admin")]
        [Route("Fitzgeralds-Flash/Edit/{id:int}")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewsFeed newsFeed = await db.NewsFeed.FindAsync(id);
            if (newsFeed == null)
            {
                return HttpNotFound();
            }
            return View(newsFeed);
        }

        // POST: NewsFeeds/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("Fitzgeralds-Flash/Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "NewsFeedID,Category")] NewsFeed newsFeed)
        {
            if (ModelState.IsValid)
            {
                db.Entry(newsFeed).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(newsFeed);
        }

        // GET: NewsFeeds/Delete/5
        [Authorize(Roles = "Admin")]
        [Route("Fitzgeralds-Flash/Delete/{id:int}")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewsFeed newsFeed = await db.NewsFeed.FindAsync(id);
            if (newsFeed == null)
            {
                return HttpNotFound();
            }
            return View(newsFeed);
        }

        // POST: NewsFeeds/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [Route("Fitzgeralds-Flash/Delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            NewsFeed newsFeed = await db.NewsFeed.FindAsync(id);
            db.NewsFeed.Remove(newsFeed);
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
