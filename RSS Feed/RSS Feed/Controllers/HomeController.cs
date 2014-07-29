using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RSS_Feed.Controllers
{
    public class HomeController : Controller
    {
        private static bool loginFlag;
        public ActionResult Index()
        {
            //Any time that the user is authenticated, we want to display the application instead
            //of the home page.
            if (!User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                ViewBag.returnUrl = "~/Views/App/Application.cshtml";
                return View("~/Views/App/Application.cshtml");
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public static void setLoginFlag(bool flag)
        {
            loginFlag = flag;
        }
    }
}