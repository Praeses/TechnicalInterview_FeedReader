using FeedReader.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FeedReader.UI.Extenstions
{
    public static class FeedUrlExtensions
    {
        public static List<string> GetUrlFromFeedUrl(this IEnumerable<FeedUrl> feedUrls)
        {
            var urls = new List<string>();
            foreach(var feed in feedUrls)
            {
                urls.Add(feed.Url);
            }
            return urls;
        }
    }
}