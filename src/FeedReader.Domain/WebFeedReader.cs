using System;
using System.Xml.Linq;

namespace FeedReader.Domain
{
    class WebFeedReader : IFeedReader
    {
        public bool LoadFeedItems(Feed f)
        {
            try
            {
                var xmlDoc = XDocument.Load(f.Url);
                
                foreach (var item in xmlDoc.Descendants("item"))
                {
                    var feedItem = new FeedItem();

                    var sourceElement = item.Element("source");
                    if (sourceElement != null)
                        feedItem.Source = sourceElement.Value;

                    if (string.IsNullOrEmpty(feedItem.Source))
                        feedItem.Source = f.Name;

                    var titleElement = item.Element("title");
                    if (titleElement != null)
                        feedItem.Title = titleElement.Value;

                    var linkElement = item.Element("link");
                    if (linkElement != null)
                        feedItem.Link = linkElement.Value;

                    var descrElement = item.Element("description");
                    if (descrElement != null)
                        feedItem.Description = descrElement.Value;

                    DateTime date;
                    var pubDateElement = item.Element("pubDate");
                    if (pubDateElement != null && DateTime.TryParse(pubDateElement.Value, out date))
                        feedItem.PublishDate = date;

                    f.FeedItems.Add(feedItem);

                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
