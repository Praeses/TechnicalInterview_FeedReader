using FeedReader.Domain;
using FeedReader.Models;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace FeedReader.Controllers
{
    [Authorize]
    public class FeedController : Controller
    {
        private readonly IFeedReaderDataSource _ds;

        public FeedController(IFeedReaderDataSource db)
        {
            _ds = db;
        }

        public ActionResult Index(string s)
        {
            var model = new FeedViewModel
            {
                Subscriptions = _ds.GetUserFeeds(User.Identity.GetUserId()),
                NewsFeed = _ds.GetFeeds(User.Identity.GetUserId(), s),
                Feeds = _ds.Feeds
            };

            return View(model);
        }

        
        public ActionResult AddFeed(string name, string url)
        {
            var feed = new Feed
            {
                Name = name,
                Url = url
            };

            _ds.AddFeed(feed);

            var model = new FeedViewModel
            {
                Subscriptions = _ds.GetUserFeeds(User.Identity.GetUserId()),
                NewsFeed = _ds.GetFeeds(User.Identity.GetUserId(), null),
                Feeds = _ds.Feeds
            };

            return PartialView("_SubscriptionsList", model);
        }
        
        public ActionResult Subscribe(Feed feed)
        {
            var subscription = new Subscription
            {
                UserId = User.Identity.GetUserId(),
                FeedId = feed.Id
            };

            _ds.AddSubscription(subscription);

            var model = new FeedViewModel
            {
                Subscriptions = _ds.GetUserFeeds(User.Identity.GetUserId()),
                NewsFeed = _ds.GetFeeds(User.Identity.GetUserId(), null),
                Feeds = _ds.Feeds
            };

            return PartialView("_SubscriptionsList", model);

        }

        public ActionResult Unsubscribe(Feed feed)
        {
            Subscription sub = null;
            foreach (var s in _ds.Subscriptions)
            {
                if (s.UserId == User.Identity.GetUserId() && s.FeedId == feed.Id)
                    sub = s;
            } 

            if(sub!=null)
                _ds.RemoveSubscription(sub);

            var model = new FeedViewModel
            {
                Subscriptions = _ds.GetUserFeeds(User.Identity.GetUserId()),
                NewsFeed = _ds.GetFeeds(User.Identity.GetUserId(), null),
                Feeds = _ds.Feeds
            };

            return PartialView("_SubscriptionsList", model);
        }
        
        public ActionResult Get(Feed feed)
        {
            var model = new FeedViewModel
            {
                Subscriptions = _ds.GetUserFeeds(User.Identity.GetUserId()),
                Feeds = _ds.Feeds
            };
            
            if (feed.Id > 0)
                model.NewsFeed = _ds.GetFeed(feed);
            else
                model.NewsFeed = _ds.GetFeeds(User.Identity.GetUserId(), null);

            return PartialView("_FeedViewer", model);
        }

        public ActionResult ShowFeed(Feed feed, string searchString)
        {
            var model = new FeedViewModel
            {
                Subscriptions = _ds.GetUserFeeds(User.Identity.GetUserId()),
                Feeds = _ds.Feeds
            };

           if (feed.Id > 0)
               model.NewsFeed = _ds.GetFeed(feed);
           else
               model.NewsFeed = _ds.GetFeeds(User.Identity.GetUserId(), searchString);

           if (Request.IsAjaxRequest())
                return PartialView("_FeedViewer", model);
            
           return RedirectToAction("Index", model); 
        }
       
    }
}