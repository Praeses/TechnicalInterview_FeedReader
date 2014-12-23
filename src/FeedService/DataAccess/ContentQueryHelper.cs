using System;
using System.Collections.Generic;
using System.Linq;
using FeedService.Contract.ContentService;
using FeedService.Model;

namespace FeedService.DataAccess
{
    internal class ContentQueryHelper
    {
        public List<SubscriptionSynchronization> GetSubscriptionSynchronization(ContentEntities context, int accountId, DateTime refreshThreshold)
        {
            var nowUtc = DateTime.UtcNow;
            var subscriptionsQuery = (from s in context.Subscriptions
                                      where s.AccountId == accountId
                                            && !s.EndDateUtc.HasValue
                                            && s.LastRefreshedUtc < refreshThreshold
                                      select new SubscriptionSynchronization
                                          {
                                              SubscriptionId = s.SubscriptionId,
                                              ResourceUri = s.ResourceUri,
                                              ItemRetentionInDays = s.ItemRetentionInDays,
                                              Subscription = s
                                          });
            //System.Diagnostics.Debug.WriteLine(subscriptionsQuery.ToString()); 
            var subscriptions = subscriptionsQuery.ToList();
            foreach (var sync in subscriptions)
            {
                var subscriptionId = sync.SubscriptionId;
                var currentItems = (from si in context.SubscriptionItems
                                    where si.SubscriptionId == subscriptionId 
                                    && si.ExpirationDateUtc > nowUtc
                                    select si.ItemId).ToList();
                sync.CurrentItems = new HashSet<string>(currentItems);
            }
            return subscriptions;
        }

        public List<AccountFeedItem> LoadItemsForSubscription(int accountId, int subscriptionId, string searchPattern, int size)
        {
            var nowUtc = DateTime.UtcNow;
            using (var context = new ContentEntities())
            {
                if (string.IsNullOrWhiteSpace(searchPattern))
                {
                    var items = (from si in context.SubscriptionItems
                                 join s in context.Subscriptions on si.SubscriptionId equals s.SubscriptionId
                                 where si.SubscriptionId == subscriptionId
                                       && s.AccountId == accountId
                                       && si.ExpirationDateUtc > nowUtc
                                 orderby si.PublishedDateUtc descending
                                 select
                                     new AccountFeedItem
                                     {
                                         SubscriptionId = subscriptionId,
                                         SubscriptionItemId = si.SubscriptionItemId,
                                         ItemContent = si.Content
                                     }).Take(size).ToList();
                    return items;
                }
                else
                {
                    //going to be a stored proc call
                    throw new NotImplementedException();
                }
            }
        }
        public List<AccountFeedItem> LoadAllItems(int accountId, string searchPattern, int size)
        {
            var nowUtc = DateTime.UtcNow;
            using (var context = new ContentEntities())
            {
                if (string.IsNullOrWhiteSpace(searchPattern))
                {
                    var items = (from si in context.SubscriptionItems
                                 join s in context.Subscriptions on si.SubscriptionId equals s.SubscriptionId
                                 where si.ExpirationDateUtc > nowUtc
                                       && s.AccountId == accountId
                                 orderby si.PublishedDateUtc descending
                                 select new AccountFeedItem
                                 {
                                     SubscriptionId = s.SubscriptionId,
                                     SubscriptionItemId = si.SubscriptionItemId,
                                     ItemContent = si.Content
                                 }).Take(size).ToList();
                    return items;
                }
                else
                {
                    //going to be a stored proc call
                    throw new NotImplementedException();
                }
            }
        }
    }
}