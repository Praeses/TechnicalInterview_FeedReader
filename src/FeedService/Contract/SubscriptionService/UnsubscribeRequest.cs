using System.Runtime.Serialization;

namespace FeedService.Contract.SubscriptionService
{
    [DataContract]
    public class UnsubscribeRequest
    {
        [DataMember]
        public int AccountId { get; set; }

        [DataMember]
        public int SubscriptionId { get; set; }
    }
}