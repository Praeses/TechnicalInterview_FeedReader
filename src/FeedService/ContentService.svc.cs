using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FeedService.Contract;
using FeedService.Contract.ContentService;
using FeedService.DataAccess;
using FeedService.Model;

namespace FeedService
{
    public class ContentService : IContentService
    {
        public LoadFeedResult LoadItemFeed(LoadFeedRequest request)
        {
            var queryHelper = new ContentQueryHelper();
            var result = new LoadFeedResult();
            try
            {
                //load items
                switch (request.Mode)
                {
                    case FeedMode.All:
                        if (request.LastSubscriptionItemId > 0)
                        {
                            result.Items = queryHelper.LoadAllItems(request.AccountId, request.SearchPattern,
                                                                    request.LastSubscriptionItemId, request.FetchSize);
                        }
                        else
                        {
                            result.Items = queryHelper.LoadAllItems(request.AccountId, request.SearchPattern, request.FetchSize);
                        }
                        break;
                    case FeedMode.Subscription:
                        if (request.LastSubscriptionItemId > 0)
                        {
                            result.Items = queryHelper.LoadItemsForSubscription(request.AccountId, request.SubscriptionId,
                                                                                request.SearchPattern,
                                                                                request.LastSubscriptionItemId,
                                                                                request.FetchSize);
                        }
                        else
                        {
                            result.Items = queryHelper.LoadItemsForSubscription(request.AccountId, request.SubscriptionId,
                                                                                request.SearchPattern,
                                                                                request.FetchSize);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                result.Code = ResultCode.Success;
            }
            catch (Exception ex)
            {
                result.Code = ResultCode.Failure;
                result.Message = ex.ToString();
            }
            return result;
        }

        public async Task<Result> SynchronizeSubscriptions(int accountId)
        {
            var result = new Result();
            var refreshThreshold = DateTime.UtcNow.AddMinutes(-5);
            var feedHelper = new FeedHelper();
            var queryHelper = new ContentQueryHelper();
            try
            {
                using (var context = new ContentEntities())
                {
                    //sync items on all active subscriptions for account
                    var subscriptions = queryHelper.GetSubscriptionSynchronization(context, accountId, refreshThreshold);
                    await feedHelper.DownloadFeeds(context, subscriptions);
                    subscriptions.ForEach(tmp=> tmp.Subscription.LastRefreshedUtc = DateTime.UtcNow);
                    context.SaveChanges();
                    result.Code = ResultCode.Success;
                }
            }
            catch (Exception ex)
            {
                result.Code = ResultCode.Failure;
                result.Message = ex.ToString();
            }
            return result;
        }

        
    }
}
