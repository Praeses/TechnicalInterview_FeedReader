using System.Web.Mvc;

namespace FeedReader.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Feed");

            return View();
        }
    }
}