using FeedReader.Models.Interfaces;
using FeedReader.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FeedReader.Data
{
    public class FeedHelper : IXmlParser
    {
        //Probably could have just used the XmlSerializer for this
        public Feed CreateFeedFromXml(XDocument rss)
        {
            Feed feed = new Feed();
            var root = rss.Root.Element("channel");
            var items = root.Descendants("item");

            feed.Title = root.Element("title").Value;
            feed.Url = root.Element("link").Value;
            feed.Description = root.Element("description").Value;

            if (root.Element("lastBuildDate") != null)
            {
                feed.LastBuildDate = DateTime.Parse(root.Element("lastBuildDate").Value);
            }

            var image = root.Element("image");
            if (image != null)
            {
                feed.Image.Url = image.Element("url").Value;
                feed.Image.Link = image.Element("link").Value;

                //If there was no image in the root, check feedItems description element until finding
                //one with an <img> tag that has a height/width > 1 and add the url for that image
                //as the feeds image. (This should just grab the last image uploaded to the feed so it
                //may or may not be relevant.)
            }

            if (feed.LastBuildDate == null)
            {
                feed.LastBuildDate = DateTime.Parse(items.ElementAt(0).Element("pubDate").Value);
            }

            foreach (var item in items)
            {
                var feedItem = new FeedItem();
                feedItem.Title = item.Element("title").Value;
                feedItem.Description = item.Element("description").Value;
                feedItem.Url = item.Element("link").Value;
                if (item.Element("guid") != null)
                {
                    feedItem.Guid = item.Element("guid").Value;
                }
                feedItem.PubDate = DateTime.Parse(item.Element("pubDate").Value);

                if (item.Descendants("media").Any())
                {
                    feedItem.MediaSrc = item.Descendants("media").Select(x => x.Attribute("url").Value).ToList();
                }

                feed.FeedItems.Add(feedItem);
            }

            return feed;
        }
    }
}
