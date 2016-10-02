using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FeedReader.Models;
using System.Xml;
using System.Xml.XPath;
using System.Text.RegularExpressions;
using System.Net;

using System.ServiceModel.Syndication;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Xml.Linq;
using System.Globalization;
using System.Reflection;

namespace FeedReader.Providers
{
    public interface IRssUpdater
    {
        RssChannel RetrieveChannel(String url);
    }

    public class XDocumentRssReader : IRssUpdater
    {
        public RssChannel RetrieveChannel(string url)
        {
            XDocument doc = XDocument.Load(url);

            string rootName = doc.Root.Name.LocalName;

            RssChannel rssChannel = null;
            switch (rootName)
            {
                case "feed":
                    rssChannel = parseAtom(doc);
                    break; 
                case "rss":
                    rssChannel = parseRss(doc);
                    break;
            }

            if (rssChannel != null)
            {
                rssChannel.FeedUrl = url;
            }

            return rssChannel;
        }

        public RssChannel parseAtom(XDocument doc)
        {
            RssChannel rssChannel = new RssChannel();

            rssChannel.Title = GetValue("title", doc.Root);
            rssChannel.ImageUrl = GetValue("icon", doc.Root);

            var entries = from entry in doc.Root.Elements().Where(d => d.Name.LocalName == "entry")
                          select entry;

            foreach (var entry in entries)
            {
                RssItem rssItem = new RssItem();

                var potentialLink = GetSubElement("link", entry);
                rssItem.Link = GetAttributeValue("href", potentialLink);

                rssItem.Title = GetValue("title", entry);
                rssItem.Description = GetValue("content", entry);

                //pull date
                rssItem.PubDate = ConvertToDateTimeOffset(GetValue("published", entry));

                rssChannel.Items.Add(rssItem);
            }

            return rssChannel;
        }

        public RssChannel parseRss(XDocument doc)
        {
            string version = GetAttributeValue("version", doc.Root);
    
            RssChannel rssChannel = new RssChannel();

            var channels = from item in doc.Root.Elements("channel")
                           select item;
            var items = from item2 in channels.Elements("item")
                        select item2;
            var channel = channels.FirstOrDefault();

            if (channel != null)
            {
                rssChannel = new RssChannel();

                rssChannel.Title = GetValue("title", channel);

                var potentialImage = GetSubElement("image", channel);
                if (potentialImage != null)
                {
                    rssChannel.ImageUrl = GetValue("url", potentialImage);
                }

                rssChannel.Description = GetValue("description", channel);

                if (items != null)
                {
                    foreach (var item in items)
                    {
                        RssItem rssItem = new RssItem();
                        rssItem.Link = GetValue("link", item);
                        rssItem.Title = GetValue("title", item);
                        rssItem.Description = GetValue("description", item);

                        string url = GetAttributeValue("url", GetSubElement("thumbnail", item));

                        if (url == null)
                        {
                            url = GetAttributeValue("url", GetSubElement("group", item));
                        }

                        rssItem.ImageUrl = url;
                        //pull date
                        rssItem.PubDate = ConvertToDateTimeOffset(GetValue("pubDate", item));

                        rssChannel.Items.Add(rssItem);
                    }
                }
            }

            return rssChannel;
        }

        private DateTimeOffset ConvertToDateTimeOffset(string dateVal)
        {

            DateTime dateTime;
            try
            {
                dateTime = DateTime.Parse(dateVal);
            }
            catch (FormatException)
            {
                Debug.WriteLine("Unable to parse date because of bad format " + dateVal);
                dateTime = DateTime.Now;
            }

            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }

        private string GetAttributeValue(string name, XElement element)
        {
            string value = null;

            if (element != null)
            {
                XAttribute potentialAttribute = element.Attribute(name);

                if (potentialAttribute != null)
                {
                    value = potentialAttribute.Value;
                }
            }
            return value;
        }

        private XElement GetSubElement(string name, XElement element)
        {
            XElement returnElement = null;
            if (element != null)
            {
                returnElement = element.Elements().Where(d => d.Name.LocalName == name).FirstOrDefault();
            }

            return returnElement;
        }

        private string GetValue(string name, XElement element)
        {
            string value = null;

            if (element != null)
            {
                var potentialElement = element.Elements().Where(d => d.Name.LocalName == name).FirstOrDefault();

                if (potentialElement != null)
                {
                    value = potentialElement.Value;
                }
            }
            return value;
        }
    }

    public class SyndicationRssUpdater : IRssUpdater
    {
        public RssChannel RetrieveChannel(string url)
        {
            RssChannel channel = null;
            try
            {
                WebClient client = new WebClient();

                XmlReader reader = new SyndicationFeedXmlReader(client.OpenRead(url));
                SyndicationFeed feed = SyndicationFeed.Load(reader);

                reader.Close();

                channel = new RssChannel();

                channel.Link = feed.Links.FirstOrDefault().Uri.ToString();
                channel.Title = feed.Title.Text;
                channel.Description = feed.Description != null ? feed.Description.Text : String.Empty;
                channel.ImageUrl = feed.ImageUrl != null ? feed.ImageUrl.ToString() : String.Empty;
                channel.FeedUrl = url;
                foreach (SyndicationItem item in feed.Items)
                {
                    RssItem rssItem = new RssItem();
                    rssItem.Link = item.Links.FirstOrDefault().Uri.ToString();
                    rssItem.Title = item.Title.Text;
                    rssItem.Description = item.Summary != null ? item.Summary.Text : String.Empty;

                    //pull date
                    rssItem.PubDate = item.PublishDate;

                    //pull images
                    SyndicationElementExtension extension = item.ElementExtensions.Where<SyndicationElementExtension>(x => x.OuterNamespace == "http://search.yahoo.com/mrss/" && x.OuterName == "thumbnail").FirstOrDefault();

                    if (extension != null)
                    {
                        XPathNavigator navigator = new XPathDocument(extension.GetReader()).CreateNavigator();
                        XmlNamespaceManager resolver = new XmlNamespaceManager(navigator.NameTable);
                        resolver.AddNamespace("media", "http://search.yahoo.com/mrss/");

                        string thumbnailUri = navigator.SelectSingleNode("media:thumbnail", resolver).GetAttribute("url", "");
                        rssItem.ImageUrl = thumbnailUri;

                    }

                    extension = item.ElementExtensions.Where<SyndicationElementExtension>(x => x.OuterNamespace == "http://search.yahoo.com/mrss/" && x.OuterName == "group").FirstOrDefault();

                    if (extension != null)
                    {
                        XPathNavigator navigator = new XPathDocument(extension.GetReader()).CreateNavigator();
                        XmlNamespaceManager resolver = new XmlNamespaceManager(navigator.NameTable);
                        resolver.AddNamespace("media", "http://search.yahoo.com/mrss/");

                        string thumbnailUri = navigator.SelectSingleNode("//media:content", resolver).GetAttribute("url", "");
                        rssItem.ImageUrl = thumbnailUri;
                    }
                    channel.Items.Add(rssItem);
                }

                //channel.Items = channel.Items.OrderByDescending(o =>o.PubDate).ToList();
                channel.Items.OrderByDescending(a=>a.PubDate);
            }
            catch (System.Net.WebException e)
            {
                Console.WriteLine(e.Message);
            }

            return channel;
        }
    }
    
}