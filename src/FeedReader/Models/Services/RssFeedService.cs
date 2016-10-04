﻿using System;
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
                Url = rssFeedUrl,
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
                String dateString = (string)item.Element("pubDate") ?? "";

                //Remove all html elements
                description = Regex.Replace(description, "<.*?>", String.Empty);

                //Shorten to max length
                if (description.Length > MaxDescriptionLength)
                {
                    description = description.Substring(0, MaxDescriptionLength) + "...";
                }
                rssFeedItem.Description = description;

                //Try to parse date for feed item
                string dateFormatFirst = "ddd, dd MMM yyyy HH:mm:ss zzz";
                string dateFormatSecond = "ddd, dd MMM yyyy HH:mm:ss Z";
                DateTime date;

                //Attempt parse with zzz
                if (!DateTime.TryParseExact(dateString, dateFormatFirst, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                {
                    //Attempt parse with Z
                    if (!DateTime.TryParseExact(dateString, dateFormatSecond, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                    {
                        //If we get here just use the current time in zzz format
                        dateString = DateTime.Now.ToString(dateFormatFirst);
                        date = DateTime.ParseExact(dateString, dateFormatFirst, CultureInfo.InvariantCulture);
                    }
                }
                rssFeedItem.PublishDate = date;
                rssFeedItem.PublishDateString = date.ToString();

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
