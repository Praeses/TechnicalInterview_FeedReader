using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NewsReader.Models;
using System.ServiceModel.Syndication;
using System.Xml;

namespace NewsReader.Controllers
{
    public class NewsController : Controller
    {
        private NewsDBContext db = new NewsDBContext();

        // GET: /News/
        public ActionResult Index()
        {
            return View(db.NewsDB.ToList());
        }

        // GET: /News/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewsItem newsitem = db.NewsDB.Find(id);
            if (newsitem == null)
            {
                return HttpNotFound();
            }
            return View(newsitem);
        }

        // GET: /News/Create
        public ActionResult Create()
        {
            NewsItem newsitem = new NewsItem();
            SyndicationFeed newsItems = SyndicationFeed.Load(XmlReader.Create("http://rss.cnn.com/rss/cnn_topstories.rss"));
            foreach (var anews in newsItems.Items.ToList())
            {
                newsitem.Title = anews.Title.Text;
                newsitem.Links = anews.Links[0].Uri.ToString();
                newsitem.PublishDate = anews.PublishDate.DateTime;
                newsitem.Summary = anews.Summary.Text;

                db.NewsDB.Add(newsitem);
                db.SaveChanges();
            }
 
            return RedirectToAction("Index");
            
        }

        // POST: /News/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,Title,Links,PublishDate,Summary")] NewsItem newsitem)
        {
            if (ModelState.IsValid)
            {
                db.NewsDB.Add(newsitem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(newsitem);
        }

        // GET: /News/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewsItem newsitem = db.NewsDB.Find(id);
            if (newsitem == null)
            {
                return HttpNotFound();
            }
            return View(newsitem);
        }

        // POST: /News/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,Title,Links,PublishDate,Summary")] NewsItem newsitem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(newsitem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(newsitem);
        }

        // GET: /News/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewsItem newsitem = db.NewsDB.Find(id);
            if (newsitem == null)
            {
                return HttpNotFound();
            }
            return View(newsitem);
        }

        // POST: /News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NewsItem newsitem = db.NewsDB.Find(id);
            db.NewsDB.Remove(newsitem);
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
