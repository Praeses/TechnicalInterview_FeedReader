using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FeedReader.Domain
{
    public interface IFeedReaderDataSource 
    {
        IQueryable<Feed> Feeds { get; }
        IQueryable<Subscription> Subscriptions { get; }
        
        void AddSubscription(Subscription s);
        void RemoveSubscription(Subscription s);

        void AddFeed(Feed f);

        IEnumerable<FeedItem> GetFeed(Feed feed);
        IEnumerable<FeedItem> GetFeeds(string userId, string s);

        IQueryable<Feed> GetUserFeeds(string userId);
     
        void Save();
    }
}
