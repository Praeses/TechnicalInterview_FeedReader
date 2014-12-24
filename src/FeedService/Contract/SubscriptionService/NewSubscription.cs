using System.Runtime.Serialization;

namespace FeedService.Contract.SubscriptionService
{
    [DataContract]
    public class NewSubscription
    {
        [DataMember]
        public int AccountId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string ResourceUri { get; set; }

        [DataMember]
        public int PostRetentionInDays { get; set; }
    }
}