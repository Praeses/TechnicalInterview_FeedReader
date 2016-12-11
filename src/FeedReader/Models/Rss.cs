using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace FeedReader.Models
{
    public class Rss
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public DateTime? PubDate { get; set; }
        public string Source { get; set; }
    }

    public class RssReader
    {
        public static IEnumerable<Rss> GetRss(string Url)
        {
            string source = GetRssTitle(Url); 
            XDocument feedXml = XDocument.Load(Url);
            var feeds = from feed in feedXml.Descendants("item")
                        select new Rss {
                            Title = feed.Element("title").Value,
                            Link = HttpUtility.HtmlDecode(feed.Element("link").Value),
                            Description = HttpUtility.HtmlDecode(feed.Element("description").Value),
                            PubDate = (feed.Element("pubDate") != null) ? ParsePublicationDate(feed.Element("pubDate").Value) : null, 
                            Source = source
                        };

            return feeds;
        }
        
        public static string GetRssTitle(string Url)
        {
            XDocument xml = XDocument.Load(Url);
            var titles = from title in xml.Descendants("channel")
                         select title.Element("title").Value;
            return titles.FirstOrDefault();
        }

        private static DateTime? ParsePublicationDate(string PubDate)
        {
            if (String.IsNullOrEmpty(PubDate))
            {
                return null;
            }
            DateTime ret;

            string[] rssdates = { "ddd, dd MMM yyyy HH:mm:ss", "R", "r" };
            
            if (DateTime.TryParseExact(PubDate, rssdates, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out ret)) {
                return ret;
            }
            return null;
        }
    }
}