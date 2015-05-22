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
using System.Xml.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Text.RegularExpressions;

namespace FeedReader.Controllers
{
    public class FeedController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Feed
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            else{
                String userId = User.Identity.GetUserId();
                List<Feed> userFeeds = db.Feeds.Where(x => x.user_id == userId).ToList();
                return View(userFeeds);
            }
        }

        // GET: Feed/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feed feed = db.Feeds.Find(id);
            if (feed == null)
            {
                return HttpNotFound();
            }
            return View(feed);
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
        public ActionResult Create([Bind(Include = "channel,link")] Feed feed)
        {
            if (ModelState.IsValid)
            {
                feed.user_id = User.Identity.GetUserId();

                // Validate feed
                try
                {
                    var req = (HttpWebRequest)WebRequest.Create(feed.link);
                    
                    var rep = req.GetResponse();
                    var reader = XmlReader.Create(rep.GetResponseStream());

                    SyndicationFeed newFeed = SyndicationFeed.Load(reader);
                    feed.desc = newFeed.Title.Text;
                }
                catch
                {
                    
                }

                db.Feeds.Add(feed);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(feed);
        }
        
        // GET: Feed/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feed feed = db.Feeds.Find(id);
            if (feed == null)
            {
                return HttpNotFound();
            }
            return View(feed);
        }

        // POST: Feed/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,user_id,channel,link,desc")] Feed feed)
        {
            if (ModelState.IsValid)
            {
                feed.user_id = User.Identity.GetUserId();
                db.Entry(feed).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(feed);
        }

        // GET: Feed/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feed feed = db.Feeds.Find(id);
            if (feed == null)
            {
                return HttpNotFound();
            }
            return View(feed);
        }

        // POST: Feed/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Feed feed = db.Feeds.Find(id);
            db.Feeds.Remove(feed);
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

        public ActionResult showArticles(String url)
        {
            IEnumerable<FeedArticle> articles = getFeedArticles(url);
            if(articles.Any())
                return View(articles);
            else
                return RedirectToAction("articleError");
        }

        public ActionResult showAllArticles()
        {
            String userId = User.Identity.GetUserId();
            Dictionary<String, IEnumerable<FeedArticle>> channelToArticles = new Dictionary<string, IEnumerable<FeedArticle>>();

            List<Feed> channels = db.Feeds.Where(x => x.user_id == userId).ToList();

            foreach (Feed feed in channels)
            {
                IEnumerable<FeedArticle> channelArticles = getFeedArticles(feed.link);

                try
                {
                    channelToArticles.Add(feed.channel, channelArticles);
                }
                catch {/* Do not add item if an exception is thrown*/}
            }

            return View(channelToArticles);
        }

        public IEnumerable<FeedArticle> getFeedArticles(String url)
        {
            //Make type IEnumerable so we can iterate on front end
            IEnumerable<FeedArticle> articles = new List<FeedArticle>();

            //Switch to SyndicationFeed from XDocument so we can handle Atom feeds as well
            try
            {
                var req = (HttpWebRequest)WebRequest.Create(url);

                var rep = req.GetResponse();
                var reader = XmlReader.Create(rep.GetResponseStream());
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                reader.Close();
                var entries = from item in feed.Items
                              select new FeedArticle
                              {
                                  title = item.Title.Text,
                                  desc = StripHTML(item.Summary.Text),
                                  link = (item.Id == null || !Uri.IsWellFormedUriString(item.Id, UriKind.Absolute)) ? item.Links[0].Uri.ToString() : item.Id,
                                  authors = (item.Authors.FirstOrDefault() ?? new SyndicationPerson()).Name,
                                  publishDate = item.PublishDate.Date.ToShortDateString()
                              };

                articles = entries.ToList();
            }
            catch
            {
                //Dont add to the list
            }

            return articles;
        }

        public static string StripHTML(string htmlString)
        {

            string pattern = @"<(.|\n)*?>";

            return Regex.Replace(htmlString, pattern, string.Empty);
        }

        public ActionResult articleError()
        {
            return View();
        }
    }
}
