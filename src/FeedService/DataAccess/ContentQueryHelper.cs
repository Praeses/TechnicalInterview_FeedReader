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

        public List<AccountFeedItem> LoadItems(int accountId, int? subscriptionId, string searchPattern, int size)
        {
            using (var context = new ContentEntities())
            {
                if (!string.IsNullOrWhiteSpace(searchPattern))
                {
                    //add wildcards
                    searchPattern = searchPattern.Trim();
                    searchPattern = searchPattern.Replace("  ", "%");
                    searchPattern = searchPattern.Replace(" ", "%");
                    searchPattern = string.Format("%{0}%", searchPattern);
                }
                var searchResults = context.usp_SearchFeedItems(accountId, subscriptionId, searchPattern, size);
                var items = (from sr in searchResults
                             select
                                 new AccountFeedItem
                                     {
                                         SubscriptionId = sr.subscription_id,
                                         SubscriptionItemId = sr.subscription_item_id,
                                         ItemContent = sr.content
                                     }).ToList();
                return items;
            }
        }

    }
}