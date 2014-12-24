using System.Runtime.Serialization;

namespace FeedService.Contract.SubscriptionService
{
    [DataContract]
    public class AccountSubscription
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int SubscriptionId { get; set; }

        [DataMember]
        public int PostRetentionInDays { get; set; }

        [DataMember]
        public string Uri { get; set; }

        [DataMember]
        public string Summary { get; set; }
    }
}