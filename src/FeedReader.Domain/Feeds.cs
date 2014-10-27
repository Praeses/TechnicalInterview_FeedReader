using System.Collections.Generic;

namespace FeedReader.Domain
{
    public class Feed
    {
        private readonly IFeedReader _reader;

        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Url { get; set; }

        public List<FeedItem> FeedItems;


        public Feed()
        {
            FeedItems = new List<FeedItem>();
            _reader = new WebFeedReader();
        }

        public Feed(IFeedReader reader)
        {
            FeedItems = new List<FeedItem>();
            _reader = reader;
        }

        public IEnumerable<FeedItem> GetFeed()
        {
            _reader.LoadFeedItems(this);
            return FeedItems;
        }
    }
}
