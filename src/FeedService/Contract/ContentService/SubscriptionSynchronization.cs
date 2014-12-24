using System;
using System.Collections.Generic;
using FeedService.Model;

namespace FeedService.Contract.ContentService
{
    public class SubscriptionSynchronization
    {
        public int SubscriptionId { get; set; }
        public string ResourceUri { get; set; }
        public int ItemRetentionInDays { get; set; }
        public Subscription Subscription { get; set; }
        public HashSet<string> CurrentItems { get; set; }
        public DateTime GetNewItemExpiration()
        {
            return DateTime.UtcNow.AddDays(ItemRetentionInDays);
        }
    }
}