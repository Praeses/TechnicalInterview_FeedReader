using FeedReader.Models;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace FeedReader.Controllers
{
    public class HomeController : Controller
    {

        private UserContext db = new UserContext();
        public ActionResult Index()
        {
            return View();
        }
    }
}