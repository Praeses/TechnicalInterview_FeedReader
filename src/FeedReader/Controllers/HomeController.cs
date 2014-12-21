using FeedReader.Models;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;


namespace FeedReader.Controllers
{
    public class HomeController : Controller
    {
        private UserSubscriptionDBContext db = new UserSubscriptionDBContext();
        public ActionResult Index()
        {
            string username = User.Identity.GetUserName();//FIXME: Not sure if this is safe?
            var subscriptions = from s in db.UserSubscriptions
                                select s;
            subscriptions = subscriptions.Where(n => n.userName.Equals(username));
            return View(subscriptions);
        }
    }
}