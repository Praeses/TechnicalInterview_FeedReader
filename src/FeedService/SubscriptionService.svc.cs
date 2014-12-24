using System;
using System.Linq;
using FeedService.DataAccess;
using FeedService.Model;
using FeedService.Contract;
using FeedService.Contract.SubscriptionService;

namespace FeedService
{
    public class SubscriptionService : ISubscriptionService
    {
        public LoadSubscriptionsResult LoadSubscriptions(int accountId)
        {
            var result = new LoadSubscriptionsResult();
            try
            {
                using (var context = new ContentEntities())
                {
                    //lookup provider
                    var subscriptions = (from s in context.Subscriptions
                                         where s.AccountId == accountId
                                         && !s.EndDateUtc.HasValue
                                         select
                                             new AccountSubscription
                                                 {
                                                     Name = s.Name,
                                                     PostRetentionInDays = s.ItemRetentionInDays,
                                                     SubscriptionId = s.SubscriptionId,
                                                     Uri = s.ResourceUri,
                                                     Summary = s.Summary
                                                 }).ToList();
                    result.Subscriptions = subscriptions;
                    result.Code = ResultCode.Success;
                }
            }
            catch (Exception ex)
            {
                result.Code = ResultCode.Failure;
                result.Message = ex.ToString();
                result.DisplayMessage = "Subscription lookup failed due to internal error";
            }

            return result;
        }

        public Result Subscribe(NewSubscription subscription)
        {
            var result = new Result();
            try
            {
                var feedHelper = new FeedHelper();
                Subscription newSubscription;
                try
                {
                    newSubscription = feedHelper.BuildSubscription(subscription);
                }
                catch (Exception ex)
                {
                    result.Code = ResultCode.Failure;
                    result.Message = ex.ToString();
                    result.DisplayMessage = "Could not add subscription. Requested URI is not valid";
                    return result;
                }
                if (newSubscription != null)
                {
                    using (var context = new ContentEntities())
                    {
                        context.Subscriptions.Add(newSubscription);
                        context.SaveChanges();
                        result.Code = ResultCode.Success;
                    }
                }
                else
                {
                    result.Code = ResultCode.Failure;
                    result.Message = "Could not add subscription. Requested URI is not valid";
                    result.DisplayMessage = "Could not add subscription. Requested URI is not valid";
                }
            }
            catch (Exception ex)
            {
                result.Code = ResultCode.Failure;
                result.Message = ex.ToString();
                result.DisplayMessage = "New subscription failed due to internal error";
            }

            return result;
        }
        public Result Unsubscribe(UnsubscribeRequest request)
        {
            var result = new Result();
            try
            {
                using (var context = new ContentEntities())
                {
                    //inactivate subscription
                    var subscription =
                        context.Subscriptions.FirstOrDefault(
                            tmp =>
                            tmp.AccountId == request.AccountId && tmp.SubscriptionId == request.SubscriptionId &&
                            !tmp.EndDateUtc.HasValue);
                    if (subscription != null)
                    {
                        subscription.EndDateUtc = DateTime.UtcNow;
                        context.SaveChanges();
                        result.Code = ResultCode.Success;
                    }
                    else
                    {
                        result.Code = ResultCode.Failure;
                        result.Message = "Subscription not found";
                        result.DisplayMessage = "Removing subscription failed because the subscription could not be found";
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = ResultCode.Failure;
                result.Message = ex.ToString();
                result.DisplayMessage = "Removing subscription failed due to internal error";
            }

            return result;

        }
    }
}
