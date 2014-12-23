using System.Runtime.Serialization;

namespace FeedService.Contract.ContentService
{
    [DataContract]
    public class LoadFeedRequest
    {
        [DataMember]
        public int AccountId { get; set; }
        [DataMember]
        public FeedMode Mode { get; set; }
        [DataMember]
        public int SubscriptionId { get; set; }
        [DataMember]
        public string SearchPattern { get; set; }
        [DataMember]
        public int FetchSize { get; set; }
        [DataMember]
        public int LastSubscriptionItemId { get; set; }
    }
}