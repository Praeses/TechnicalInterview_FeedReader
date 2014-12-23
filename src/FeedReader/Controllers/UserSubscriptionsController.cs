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
using System.Xml;
using System.ServiceModel.Syndication;

namespace FeedReader.Controllers
{
    public class UserSubscriptionsController : Controller
    {
        private UserSubscriptionDBContext db = new UserSubscriptionDBContext();

        // GET: UserSubscriptions
        public ActionResult Index(string searchString)
        {
            var subscriptions = getAllSubscriptionsForUser();
            
            if(!String.IsNullOrEmpty(searchString))
            {
                subscriptions = subscriptions.Where(n => n.rssFeedURL.Contains(searchString));
            }

            return View(subscriptions);
        }

        // GET: UserSubscriptions/Details/5
        public ActionResult Details(int? id)//TODO: Add search string (similar to Index())
        {
            UserSubscription userSubscription;
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                
            }

            userSubscription = db.UserSubscriptions.Find(id);
            if (userSubscription == null)
            {
                return HttpNotFound();
            }

            //WebClient client = new WebClient();
            //client.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.835.202 Safari/535.1";
            //client.Headers["Accept"] = "xml;q=0.9, */*;q=0.8";
            //string data = client.DownloadString(userSubscription.rssFeedURL);
            //XmlDocument doc = new XmlDocument();
            //doc.LoadXml(data);

            //Consider Removing
            XmlReader reader = XmlReader.Create(userSubscription.rssFeedURL);
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            reader.Close();

            if(feed != null)
            {
                var item = (from rss in feed.Items
                            select new RSSFeed
                            {
                                id = rss.Id,
                                title = rss.Title.Text,
                                description = rss.Summary.Text,
                                link = rss.Links.Count > 0 ? rss.Links.First().Uri : null,//FIXME: Not sure if this is correct//FIXME: Consider making this a list of some sort
                                category = rss.Categories.Count > 0 ? rss.Categories.First().Label : "",//FIXME: Not sure if this is correct//FIXME: Consider making this a list of some sort
                                publishDate = rss.PublishDate.DateTime
                            });
                //return View(userSubscription);
                return View(item);
            }
            //End Remove
            return View();
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

        //GET: UserSubscriptions/_Feed
        public ActionResult _Feed(int? id, string searchString)//TODO: Handle searching via the search string
        {
            UserSubscription userSubscription;
            if(id != null)
            {
                userSubscription = db.UserSubscriptions.Find(id);

                XmlReader reader = XmlReader.Create(userSubscription.rssFeedURL);
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                reader.Close();

                if (feed != null)
                {
                    var item = (from rss in feed.Items
                                select new RSSFeed
                                {
                                    id = rss.Id,
                                    title = rss.Title.Text,
                                    description = rss.Summary.Text,
                                    link = rss.Links.Count > 0 ? rss.Links.First().Uri : null,//FIXME: Not sure if this is correct//FIXME: Consider making this a list of some sort (IEnumerable perhaps)
                                    category = rss.Categories.Count > 0 ? rss.Categories.First().Label : "",//FIXME: Not sure if this is correct//FIXME: Consider making this a list of some sort (IEnumerable perhaps)
                                    publishDate = rss.PublishDate.DateTime
                                });
                    return PartialView(item);
                }
            }
            return PartialView(); 
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private IEnumerable<UserSubscription> getAllSubscriptionsForUser()
        {
            string username = User.Identity.GetUserName();//FIXME: Not sure if this is safe?
            var subscriptions = from s in db.UserSubscriptions
                                select s;
            subscriptions = subscriptions.Where(n => n.userName.Equals(username));
            return subscriptions;
        }
    }
}
