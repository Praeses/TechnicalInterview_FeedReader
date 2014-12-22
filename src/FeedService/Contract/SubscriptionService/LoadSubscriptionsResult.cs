using FeedService.Model;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FeedService.Contract.SubscriptionService
{
    [DataContract]
    public class LoadSubscriptionsResult : Result
    {
        public LoadSubscriptionsResult()
        {
            Subscriptions = new List<AccountSubscription>();
        }
        [DataMember]
        public List<AccountSubscription> Subscriptions { get; set; }
    }
}