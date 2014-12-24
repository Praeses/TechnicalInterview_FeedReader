using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FeedReader.ContentService;
using FeedReader.Models;
using FeedReader.SubscriptionService;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using log4net;
using ResultCode = FeedReader.SubscriptionService.ResultCode;

namespace FeedReader.Controllers
{
    [Authorize]
    public class FeedController : Controller
    {
        protected ILog Log
        {
            get { return LogManager.GetLogger("FeedController"); }
        }
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
            if (user == null)
            {
                ModelState.AddModelError("", "The user either does not exist or is not confirmed.");
                return View(new FeedModel());
            }
            var model = new FeedModel();
            model.MenuItems = GetSubscriptions(user.AccountId);
            UpdateSubscriptions(user.AccountId);
            var client = new ContentServiceClient();
            var request = new LoadFeedRequest { AccountId = user.AccountId, FetchSize = 25 };
            var result = client.LoadItemFeed(request);
            if (result.Code == ContentService.ResultCode.Success)
            {
                model.DetailOptions = new DetailOptions();
                model.DetailOptions.DisplayItems = (from fi in result.Items
                                                    join sub in model.MenuItems on fi.SubscriptionId equals sub.SubscriptionId
                                                    select new FeedItem
                                                        {
                                                            SubscriptionItemId = fi.SubscriptionItemId,
                                                            PublishedContent = fi.ItemContent,
                                                            Publisher = sub.Name
                                                        }).ToList();
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult LoadDetailPane(DetailOptions options)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var client = new ContentServiceClient();
            var request = new LoadFeedRequest {AccountId = user.AccountId, FetchSize = 25 };
            switch (options.Mode)
            {
                case ViewMode.Subscription:
                    request.Mode = FeedMode.Subscription;
                    request.SubscriptionId = options.SubscriptionId;
                    break;
                default:
                    request.Mode = FeedMode.All;
                    break;
            }
            if (!string.IsNullOrWhiteSpace(options.SearchPattern))
                request.SearchPattern = options.SearchPattern;
            var subs = GetSubscriptions(user.AccountId);
            var result = client.LoadItemFeed(request);
            var model = new FeedModel();
            model.DetailOptions = options;
            if (result.Code == ContentService.ResultCode.Success)
            {
                model.DetailOptions.DisplayItems = (from fi in result.Items
                                        join sub in subs on fi.SubscriptionId equals sub.SubscriptionId
                                      select new FeedItem
                                              {
                                                  SubscriptionItemId = fi.SubscriptionItemId,
                                                  PublishedContent = fi.ItemContent,
                                                  Publisher = sub.Name
                                              }).ToList();
            }
            model.MenuItems = GetSubscriptions(user.AccountId);
            return PartialView("Index", model);
        }

        protected void UpdateSubscriptions(int accountId)
        {
            var client = new ContentServiceClient();
            var result = client.SynchronizeSubscriptions(accountId);
            if (result.Code != ContentService.ResultCode.Success)
                ModelState.AddModelError("", "Error synchronizing feeds");
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
        protected override void OnException(ExceptionContext filterContext)
        {
            var e = filterContext.Exception;
            Log.Error("Feed Error:", e);
            filterContext.ExceptionHandled = true;
            filterContext.Result = new ViewResult()
            {
                ViewName = "Error"
            };
        }

    }
}
