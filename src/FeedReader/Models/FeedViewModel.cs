using System.Collections.Generic;
using FeedReader.Domain;

namespace FeedReader.Models
{
    public class FeedViewModel
    {
        public string SearchString { get; set; }

        public string FeedName { get; set; }
        public string FeedUrl { get; set; }

        public IEnumerable<Feed> Subscriptions { get; set; }
        public IEnumerable<FeedItem> NewsFeed { get; set; }

        public IEnumerable<Feed> Feeds { get; set; }
    }
}