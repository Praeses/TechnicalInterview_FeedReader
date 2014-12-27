using FeedReader.Models;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Web.Security;

namespace FeedReader.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index(int id = 0)
        {
            ViewBag.feed_id = id;
            ViewBag.user_id = "";
            ViewBag.search = false;

            return View();
        }

        [HttpPost]
        //Used by search
        public ActionResult Index(string needle, string current_only, int current_id)
        {
            ViewBag.feed_id = current_id;
            ViewBag.user_id = "";
            ViewBag.Search = true;
            ViewBag.needle = needle;

            if ((current_only == null || current_id == 0 || current_only.CompareTo("yes") != 0))
            {
                ViewBag.feed_id = 0;
            }

            if (string.IsNullOrEmpty(needle) == true)
            {
                return RedirectToAction("Index", new { id = current_id });
            }

            return View();
        }

        [HttpPost]
        public ActionResult Subscribe(string link, string user_id, int current_id)
        {
            if (string.IsNullOrEmpty(link) == true)
            {
                return RedirectToAction("Index", new { id = current_id });
            }
            UserFeedController ufc = new UserFeedController();
            ufc.AddUserFeed(link, user_id);

            return RedirectToAction("Index");
        }
    }
}