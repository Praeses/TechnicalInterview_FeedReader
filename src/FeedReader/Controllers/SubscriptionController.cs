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
            if (user == null || !(UserManager.IsEmailConfirmed(user.Id)))
            {
                ModelState.AddModelError("", "The user either does not exist or is not confirmed.");
                return View();
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
                var accountId = Convert.ToInt32(Session["AccountId"]);
                var newSubscription = new NewSubscription
                    {
                        AccountId = accountId,
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
                }
            }
            return RedirectToAction("Manage", "Subscription");
        }

        [HttpPost]
        public ActionResult Remove(int subscriptionId)
        {
            ValidateRequestHeader(Request);
            var client = new SubscriptionServiceClient();
            var result = client.Unsubscribe(subscriptionId);
            return Json(new {result = result.Code == ResultCode.Success});
        }

        protected void ValidateRequestHeader(HttpRequestBase request)
        {
            string cookieToken = "";
            string formToken = "";

            IEnumerable<string> tokenHeaders = request.Headers.GetValues("__RequestVerificationToken");
            if (tokenHeaders != null)
            {
                string[] tokens = tokenHeaders.First().Split(':');
                if (tokens.Length == 2)
                {
                    cookieToken = tokens[0].Trim();
                    formToken = tokens[1].Trim();
                }
            }
            AntiForgery.Validate(cookieToken, formToken);
        }

        //public ActionResult Search()
        //{
        //    return View();
        //}

    }
}
