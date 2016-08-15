using HigLabo.Net.Rss;
using Suyati.FeedAggreagator;
using System;
using System.Collections.Generic;

namespace FeedReader.Services
{
    public class CachingRssClient
    {
        private Dictionary<string, IFeed> rssFeedCache;

        public CachingRssClient()
        {
            rssFeedCache = new Dictionary<string, IFeed>();
        }

        public IFeed GetRssFeed(string url)
        {
            if (rssFeedCache.ContainsKey(url))
            {
                return rssFeedCache[url];
            }
            var feedResult = SyndicationFeed.Load(url);
            rssFeedCache.Add(url, feedResult.Feed);
            return feedResult.Feed;
        }
        
    }
}