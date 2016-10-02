using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
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
        public const int MaxDescriptionLength = 400;

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
                rssFeedItem = new RssFeedItem();
            
                rssFeedItem.Title = (string)item.Element("title") ?? "";
                rssFeedItem.Link = (string)item.Element("link") ?? "";
                rssFeedItem.Content = (string)item.Element(content + "encoded") ?? "";
                String description = (string)item.Element("description") ?? "";
                String date = (string)item.Element("pubDate") ?? "";

                //Remove all html elements
                description = Regex.Replace(description, "<.*?>", String.Empty);

                //Shorten to max length
                if (description.Length > MaxDescriptionLength)
                {
                    description = description.Substring(0, MaxDescriptionLength);
                    description += "...";
                }
                rssFeedItem.Description = description;

                if (date == "")
                    date = DateTime.Now.ToString();
                
                rssFeedItem.PublishDate = date;

                //XNamespace asdf = "http://search.yahoo.com/mrss/";
                //XElement imageElement = item.Element(asdf + "content");
                //var imageUrl = imageElement.Descendants("url");

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
