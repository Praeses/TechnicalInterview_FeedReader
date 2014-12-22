using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using FeedReader.Models;
using FeedReader.SubscriptionService;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace FeedReader.Controllers
{
    [Authorize]
    public class FeedController : Controller
    {
        private ApplicationUserManager _userManager;
        
        public FeedController()
        {
        }

        public FeedController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        
        //
        // GET: /Feed/
        public ActionResult Index()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user == null || !(UserManager.IsEmailConfirmed(user.Id)))
            {
                ModelState.AddModelError("", "The user either does not exist or is not confirmed.");
                return View();
            }
            var model = new FeedModel();
            model.MenuItems = GetSubscriptions(user.AccountId);
            return View(model);
        }

        public PartialViewResult LoadDetailPane(DetailOptions options)
        {
            var model = new DetailModel();
            model.DisplayItems = new List<FeedItem>();
            model.DisplayItems.Add(new FeedItem() { SubscriptionItemId = 500});
            return PartialView("_FeedItems", model);
        }

        protected List<MenuItem> GetSubscriptions(int accountId)
        {
            var retList = new List<MenuItem>();
            var client = new SubscriptionServiceClient();
            var result = client.LoadSubscriptions(accountId);

            if (result.Code == ResultCode.Success)
            {
                foreach (var sub in result.Subscriptions)
                {
                    var item = new MenuItem
                    {
                        Name = sub.Name, 
                        SubscriptionId = sub.SubscriptionId
                    };
                    retList.Add(item);
                }
            }

            return retList;
        }

        //public ActionResult Load()
        //{
        //    return View();
        //}

        //public ActionResult Search()
        //{
        //    return View();
        //}

        //public ActionResult Save()
        //{
        //    return View();

        //}

        //public ActionResult Remove()
        //{
        //    return View();

        //}

        //[HttpPost]
        //public ActionResult Share()
        //{
        //    return View();

        //}

    }
}
