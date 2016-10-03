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
using FeedReader.Utils;

namespace FeedReader.Providers
{
    /// <summary>
    /// Interface describing the methods that allow for retrieving an Rss feed from an external source
    /// </summary>
    public interface IRssUpdater
    {
        /// <summary>
        /// Retrieves a channel from a requested url
        /// </summary>
        /// <param name="url">Requsted feed url</param>
        /// <returns>RssChannel from the requested Url</returns>
        RssChannel RetrieveChannel(string url);
    }

    public class ChainingRssReader : IRssUpdater
    {
        private ICollection<IRssUpdater> _parseChain = new List<IRssUpdater>();

        public ChainingRssReader()
        {
            addToRssParseChain(new SyndicationRssUpdater());
            addToRssParseChain(new XDocumentRssReader());
        }
        public RssChannel RetrieveChannel(string url)
        {
            RssChannel channel = null;

            foreach(IRssUpdater updater in _parseChain)
            {
                try
                {
                    channel = updater.RetrieveChannel(url);
                }
                catch (Exception)
                {
                    Debug.WriteLine("Unable to parse feed " + url + " with parser: " + updater.ToString());
                }                

                if(channel != null){
                    break;
                }
            }

            if (channel == null)
            {
                throw new RssUpdateException();
            }
            return channel;
        }

        public void addToRssParseChain(IRssUpdater updater)
        {
            _parseChain.Add(updater);
        }
    }
    /// <summary>
    /// Manual parser for rss and atoms feed. Used in case the internal SyndicationFeed reader is unable to process the feed. 
    /// </summary>
    public class XDocumentRssReader : IRssUpdater
    {
        /// <summary>
        /// Retrieves a channel from a requested url
        /// </summary>
        /// <param name="url">Requsted feed url</param>
        /// <returns>RssChannel from the requested Url</returns>
        public RssChannel RetrieveChannel(string url)
        {
            RssChannel rssChannel = null;
            try 
            {
                WebClient client = new RssWebClient();
                XDocument doc = XDocument.Load(client.OpenRead(url));

                string rootName = doc.Root.Name.LocalName;
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
            }
            catch(Exception e)
            {
                throw new RssUpdateException(e.Message, e);
            }
            return rssChannel;
        }

        /// <summary>
        /// Parses an atom based feed
        /// </summary>
        /// <param name="doc">XML root of the atom feed</param>
        /// <returns>RssChannel from the document</returns>
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
                rssItem.GenerateHash(rssChannel);
                rssChannel.Items.Add(rssItem);
            }

            return rssChannel;
        }

        /// <summary>
        /// Parses an rss based feed
        /// </summary>
        /// <param name="doc">XML root of the rss feed</param>
        /// <returns>RssChannel from the document</returns>
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

                        rssItem.GenerateHash(rssChannel);
                        rssChannel.Items.Add(rssItem);
                    }
                }
            }

            return rssChannel;
        }

        /// <summary>
        /// Parses string dates from the feeds and sets them to the current time if it cannot.
        /// </summary>
        /// <param name="dateVal">String representation of the date</param>
        /// <returns>DateTimeOffset represetnation of the date or now if parsing fails</returns>
        private DateTimeOffset ConvertToDateTimeOffset(string dateVal)
        {
            DateTime dateTime;

            try
            {
                dateTime = DateTime.Parse(dateVal);
            }
            catch (Exception)
            {
                Debug.WriteLine("Unable to parse date because of bad format " + dateVal);
                dateTime = DateTime.Now;
            }

            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }

        /// <summary>
        /// Helper method to return a tag's attribute value by name regardless of namespace
        /// </summary>
        /// <param name="name">Name of the attribute</param>
        /// <param name="element">The element to look for the attribute in</param>
        /// <returns>string representation of the value</returns>
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
        /// <summary>
        /// Helper method to return a sub element by name regardless of namespace
        /// </summary>
        /// <param name="name">Name of the local tag</param>
        /// <param name="element">The element to look for the tag in</param>
        /// <returns>XElement if it exists or null</returns>
        private XElement GetSubElement(string name, XElement element)
        {
            XElement returnElement = null;
            if (element != null)
            {
                returnElement = element.Elements().Where(d => d.Name.LocalName == name).FirstOrDefault();
            }

            return returnElement;
        }

        /// <summary>
        /// Helper method to return a tag's value by name regardless of namespace
        /// </summary>
        /// <param name="name">Name of the element</param>
        /// <param name="element">The element to look for the local name in</param>
        /// <returns>string representation of the value</returns>
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

    /// <summary>
    /// RssUpdater the uses the .net internal SyndicationFeedReader to parse a feed
    /// </summary>
    public class SyndicationRssUpdater : IRssUpdater
    {

        /// <summary>
        /// Retrieves a channel from a requested url
        /// </summary>
        /// <param name="url">Requsted feed url</param>
        /// <returns>RssChannel from the requested Url</returns>
        public RssChannel RetrieveChannel(string url)
        {
            RssChannel channel = null;
            try
            {
                WebClient client = new RssWebClient();

                //use a customer xml reader to handle non standard dates
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
                    rssItem.GenerateHash(channel);
                    channel.Items.Add(rssItem);
                }

                //channel.Items = channel.Items.OrderByDescending(o =>o.PubDate).ToList();
                channel.Items.OrderByDescending(a=>a.PubDate);
            }
            catch (System.Net.WebException e)
            {
                throw new RssUpdateException(e.Message, e);
            }

            return channel;
        }
    }

    /// <summary>
    /// Exception to signal when an Rss feed could not be retrieved
    /// </summary>
    public class RssUpdateException : Exception
    {

        /// <summary>
        /// No arg constructor
        /// </summary>
        public RssUpdateException()
        {

        }
        /// <summary>
        /// Accepts a message for the exception
        /// </summary>
        /// <param name="message">Message to be set</param>
        public RssUpdateException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Accepts a message and anexeption object
        /// </summary>
        /// <param name="message">Message to be set</param>
        /// <param name="inner">The causing exception</param>
        public RssUpdateException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}