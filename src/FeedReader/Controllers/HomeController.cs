using FeedReader.Models;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;


namespace FeedReader.Controllers
{
    public class HomeController : Controller
    {
        private UserSubscriptionDBContext db = new UserSubscriptionDBContext();
        public ActionResult Index(string rssFeed)
        {
            string username = User.Identity.GetUserName();//FIXME: Not sure if this is safe?
            
            if(!string.IsNullOrEmpty(username))
            {
                var RssLst = new List<string>();

                var subscriptions = from s in db.UserSubscriptions
                                    select s;
                subscriptions = subscriptions.Where(n => n.userName.Equals(username));
            
                var RssQuery = from d in subscriptions
                               orderby d.rssFeedName
                               select d.rssFeedName;

                RssLst.AddRange(RssQuery.Distinct());

                ViewBag.rssFeed = new SelectList(RssLst);

                //Search by rss Feed name
                if(!string.IsNullOrEmpty(rssFeed))
                {
                    subscriptions = subscriptions.Where(f => f.rssFeedName == rssFeed);
                }
                return View(subscriptions);
            }
            return View("LoginError");
        }
    }
}