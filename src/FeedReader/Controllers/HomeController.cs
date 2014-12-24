using System.Web.Mvc;
using log4net;

namespace FeedReader.Controllers
{
    public class HomeController : Controller
    {
        protected ILog Log
        {
            get { return LogManager.GetLogger("HomeController"); }
        }
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Feed");
            }
            return View();
        }
        protected override void OnException(ExceptionContext filterContext)
        {
            var e = filterContext.Exception;
            Log.Error("Home Error:", e);
            filterContext.ExceptionHandled = true;
            filterContext.Result = new ViewResult()
            {
                ViewName = "Error"
            };
        }
    }
}