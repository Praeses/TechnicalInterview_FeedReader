using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FeedService.Contract.ContentService
{
    [DataContract]
    public class LoadFeedResult : Result
    {
        public LoadFeedResult()
        {
            Items = new List<AccountFeedItem>();
        }
        [DataMember]
        public List<AccountFeedItem> Items { get; set; }
    }
}