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

namespace FeedReader.Providers
{
    public interface IRssUpdater
    {
        RssChannel retrieveChannel(String url);
    }

    public class RssUpdater : IRssUpdater
    {
        public RssChannel retrieveChannel(string url)
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
                channel.Description = feed.Description != null ? scrubHtml(feed.Description.Text) : String.Empty;
                channel.ImageUrl = feed.ImageUrl != null ? feed.ImageUrl.ToString() : String.Empty;
                channel.FeedUrl = url;

                foreach (SyndicationItem item in feed.Items)
                {
                    RssItem rssItem = new RssItem();
                    rssItem.Link = item.Links.FirstOrDefault().Uri.ToString();
                    rssItem.Title = item.Title.Text;
                    rssItem.Description = item.Summary != null ? scrubHtml(item.Summary.Text) : String.Empty;

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

        private string scrubHtml(String html)
        {
            string noHtml = Regex.Replace(html, @"<[^>]+>|&nbsp;", "").Trim();

            return Regex.Replace(noHtml, @"\s{2,}", " ");
        }
    }
    
}