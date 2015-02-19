using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BozemansFeedReader.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Bozeman's application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Praeses Company";

            return View();
        }
    }
}