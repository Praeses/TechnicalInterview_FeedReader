using System.Runtime.Serialization;

namespace FeedService.Contract.ContentService
{
    [DataContract]
    public class AccountFeedItem
    {
        [DataMember]
        public int SubscriptionId { get; set; }
        [DataMember]
        public int SubscriptionItemId { get; set; }
        [DataMember]
        public string ItemContent { get; set; }
    }
}