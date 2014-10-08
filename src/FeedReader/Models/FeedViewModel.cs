using System.Collections.Generic;
using System.Xml.Linq;

namespace FeedReader.Models
{
    public class FeedViewModel
    {
        public string SearchString { get; set; }

        public string FeedName { get; set; }
        public string FeedUrl { get; set; }

        public IEnumerable<Domain.Feed> Subscriptions { get; set; }
        public XDocument NewsFeed { get; set; }

        public IEnumerable<Domain.Feed> Feeds { get; set; }
    }
}