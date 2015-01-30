using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FeedReader.UI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if(Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Feed");
            }
            return View();
        }
    }
}