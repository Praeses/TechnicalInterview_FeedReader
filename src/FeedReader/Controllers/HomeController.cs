using System.Web.Mvc;
using FeedReader.Models;
using System.Collections.Generic;

namespace FeedReader.Controllers
{
    public class HomeController : Controller
    {       
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Thomas Fitzgerald";

            return View();
        }       
    }
}