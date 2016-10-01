using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FeedReader.Models.Services
{
    interface IRssFeedService
    {
        RssFeed RetrieveRssFeed(string rssFeedUrl);
        List<RssFeed> RetrieveAllRssFeeds(List<string> rssFeedUrlList);
    }

    public class RssFeedService : IRssFeedService
    {
        public RssFeed RetrieveRssFeed(string rssFeedUrl)
        {
            List<RssFeedItem> rssFeedItems = new List<RssFeedItem>();
            RssFeed rssFeed;
            RssFeedItem rssFeedItem;

            //Convert rssfeed into xml document
            XNamespace content = XNamespace.Get(rssFeedUrl);
            XDocument document = XDocument.Load(rssFeedUrl);

            //Pull out channel information into RssFeed object
            var channel = document.Element("rss").Element("channel");
            rssFeed = new RssFeed
            {
                Title = (string)channel.Element("title") ?? "",
                Url = (string)channel.Element("link") ?? "",
                Description = (string)channel.Element("description") ?? ""
            };

            //Loop through items and create new RssFeedItem for each
            foreach (XElement item in document.Descendants("item"))
            {
                rssFeedItem = new RssFeedItem
                {
                    Title = (string)item.Element("title") ?? "",
                    Link = (string)item.Element("link") ?? "",
                    PublishDate = (string)item.Element("pubDate") ?? "",
                    Description = (string)item.Element("description") ?? "",
                    Content = (string)item.Element(content + "encoded") ?? ""
                };

                rssFeedItems.Add(rssFeedItem);
            }
            //Add items to RssFeed
            rssFeed.RssFeedItems = rssFeedItems;

            return rssFeed;
        }

        public List<RssFeed> RetrieveAllRssFeeds(List<string> rssFeedUrlList)
        {
            List<RssFeed> rssFeedList = new List<RssFeed>();

            return rssFeedList;
        }
    }
}
