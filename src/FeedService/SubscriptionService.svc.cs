using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
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
            }

            return result;
        }

        public Result Subscribe(NewSubscription subscription)
        {
            var result = new Result();
            try
            {
                //lookup resource
                var webClient = new WebClient();
                webClient.Headers.Add("user-agent", "myNewsstand/1.0");
                var xmlReader = XmlReader.Create(webClient.OpenRead(subscription.ResourceUri));
                var feed = SyndicationFeed.Load(xmlReader);
                if (feed != null)
                {
                    feed.Items = new List<SyndicationItem>();
                    var feedBuilder = new StringBuilder();
                    using (var feedWriter = XmlWriter.Create(feedBuilder))
                    {
                        var formatter = feed.GetAtom10Formatter();
                        formatter.WriteTo(feedWriter);
                        feedWriter.Close();
                    }

                    using (var context = new ContentEntities())
                    {
                        //add subscription
                        var newSubscription = new Subscription
                        {
                            AccountId = subscription.AccountId,
                            ResourceUri = subscription.ResourceUri,
                            Name = subscription.Name,
                            ItemRetentionInDays = 30,
                            StartDateUtc = DateTime.UtcNow,
                            ContentType = "NEWSFEED",
                            LastRefreshedUtc = new DateTime(1900, 1, 1),
                            Summary = feedBuilder.ToString()
                        };
                        context.Subscriptions.Add(newSubscription);
                        context.SaveChanges();
                        result.Code = ResultCode.Success;
                    }
                }
                else
                {
                    throw new Exception("Feed not found at specified uri");
                }
            }
            catch (Exception ex)
            {
                result.Code = ResultCode.Failure;
                result.Message = ex.ToString();
            }

            return result;
        }
        public Result Unsubscribe(int subscriptionId)
        {
            var result = new Result();
            try
            {
                using (var context = new ContentEntities())
                {
                    //inactivate subscription
                    var subscription = context.Subscriptions.Find(subscriptionId);
                    subscription.EndDateUtc = DateTime.UtcNow;
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

        public Result Search()
        {
            var result = new Result();
            try
            {
                using (var context = new AccountEntities())
                {
                }
            }
            catch (Exception ex)
            {
                result.Code = ResultCode.Failure;
                result.Message = ex.ToString();
            }

            return result;
        }
        public Result Share()
        {
            var result = new Result();
            try
            {
                using (var context = new AccountEntities())
                {
                }
            }
            catch (Exception ex)
            {
                result.Code = ResultCode.Failure;
                result.Message = ex.ToString();
            }

            return result;
        }
        public Result Notify()
        {
            var result = new Result();
            try
            {
                using (var context = new AccountEntities())
                {
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
