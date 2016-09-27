using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.XPath;
using System.ServiceModel.Syndication;
using FeedReader.Models;
using System.Text.RegularExpressions;

namespace FeedReader.Controllers
{
    public class RssController : Controller
    {
        // GET: Rss
        public ActionResult Index(Dictionary<String, Object> model)
        {
            try
            {
                XmlReader reader = XmlReader.Create("http://rss.cnn.com/rss/cnn_topstories.rss");
                SyndicationFeed feed = SyndicationFeed.Load(reader);

                reader.Close();

                RssChannel channel = new RssChannel();

                channel.Link = feed.Links.FirstOrDefault().Uri.ToString();
                channel.Title = feed.Title.Text;
                channel.Description = feed.Description != null ? scrubHtml(feed.Description.Text) : String.Empty;
                channel.ImageUrl = feed.ImageUrl != null ? feed.ImageUrl.ToString() : String.Empty;

                foreach (SyndicationItem item in feed.Items)
                {
                    RssItem rssItem = new RssItem();
                    rssItem.Link = item.Links.FirstOrDefault().Uri.ToString();
                    rssItem.Title = item.Title.Text;
                    rssItem.Description = item.Summary != null ? scrubHtml(item.Summary.Text) : String.Empty;

                    SyndicationElementExtension extension = item.ElementExtensions.Where<SyndicationElementExtension>(x => x.OuterNamespace == "http://search.yahoo.com/mrss/" && x.OuterName == "media").FirstOrDefault();

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
                model.Add("channel", channel);
            }
            catch (System.Net.WebException e)
            {
                Console.WriteLine(e.Message);
            }
           
            return View(model);
        }

        private string scrubHtml(String html)
        {
            string noHtml = Regex.Replace(html, @"<[^>]+>|&nbsp;", "").Trim();

            return Regex.Replace(noHtml, @"\s{2,}", " ");
        }
        public ActionResult AddFeed()
        {
            return View();
        }

        public ActionResult RemoveFeed()
        {
            return View();
        }

    }
}