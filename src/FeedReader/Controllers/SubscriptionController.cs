using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using FeedReader.Models;
using FeedReader.SubscriptionService;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace FeedReader.Controllers
{
    [Authorize]
    public class SubscriptionController : Controller
    {
        private ApplicationUserManager _userManager;
        
        public SubscriptionController()
        {
        }

        public SubscriptionController(ApplicationUserManager userManager)
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
        // GET: /Subscription/

        public ActionResult Manage()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user == null)
            {
                ModelState.AddModelError("", "The user either does not exist or is not confirmed.");
                return View(new ManageSubscriptionsModel());
            }
            var model = new ManageSubscriptionsModel {Subscriptions = GetSubscriptions(user.AccountId)};
            return View(model);
        }

        protected ObservableCollection<SubscriptionListItem> GetSubscriptions(int accountId)
        {
            var retList = new ObservableCollection<SubscriptionListItem>();
            var client = new SubscriptionServiceClient();
            var result = client.LoadSubscriptions(accountId);

            if (result.Code == ResultCode.Success)
            {
                foreach (var sub in result.Subscriptions)
                {
                    var item = new SubscriptionListItem
                    {
                        Name = sub.Name, SubscriptionId = sub.SubscriptionId, Uri = sub.Uri, Summary = sub.Summary
                    };
                    retList.Add(item);
                }
            }

            return retList;
        }

        public ActionResult Add()
        {
            return PartialView("_NewSubscriptionPartial");
        }

        [HttpPost]
        public ActionResult Add(ManageSubscriptionsModel model)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.FindById(User.Identity.GetUserId());
                var newSubscription = new NewSubscription
                    {
                        AccountId = user.AccountId,
                        Name = model.NewItem.Name,
                        PostRetentionInDays = 30,
                        ResourceUri = model.NewItem.Uri
                    };


                var client = new SubscriptionServiceClient();
                var result = client.Subscribe(newSubscription);
                if (result.Code != ResultCode.Success)
                {
                    if (string.IsNullOrWhiteSpace(result.Message))
                        result.Message = "Unknown error";
                    ModelState.AddModelError(string.Empty, result.Message);
                    return View(model);
                }
            }
            return RedirectToAction("Manage", "Subscription");
        }

        public ActionResult Remove(int subscriptionId)
        {
            var model = new RemoveSubscriptionModel {SubscriptionId = subscriptionId};
            return PartialView("_RemoveSubscriptionPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Remove(RemoveSubscriptionModel model)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.FindById(User.Identity.GetUserId());
                var client = new SubscriptionServiceClient();
                var request = new UnsubscribeRequest { AccountId = user.AccountId, SubscriptionId = model.SubscriptionId };
                var result = client.Unsubscribe(request);
                if (result.Code == ResultCode.Success)
                {
                    return RedirectToAction("Manage", "Subscription");
                }
            }
            return PartialView("_RemoveSubscriptionPartial", model);
        }


    }
}
