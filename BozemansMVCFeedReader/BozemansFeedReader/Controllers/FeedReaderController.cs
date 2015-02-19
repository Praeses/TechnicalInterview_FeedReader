using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BozemansFeedReader.Controllers
{
    public class FeedReaderController : Controller
    {
        // GET: FeedReader
        public ActionResult Index()
        {
            return View();
        }
    }
}