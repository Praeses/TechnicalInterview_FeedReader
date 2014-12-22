using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FeedReader.Controllers
{
    public class SubscriptionsController : Controller
    {
        // GET: Subscriptions
        public ActionResult Index()
        {
            return View();
        }

        //GET: Subscriptions/Subscriptions
        //Modify and Delete RSS Subscriptions
        public ActionResult Subscriptions(string userName)
        {
            ViewBag.Header = "Subscriptions for " + userName;
            return View();
        }

        //GET: Subscriptions/Subscribe
        public string Subscribe()
        {
            return "Temporary Subscribe";
        }
    }
}