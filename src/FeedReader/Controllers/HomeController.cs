using System.Web.Mvc;

namespace FeedReader.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}