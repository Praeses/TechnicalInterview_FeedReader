using System.Data.Entity.Migrations;
using System.Globalization;
using System.Web.UI.WebControls;
using FeedReader.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Xml.Linq;

namespace FeedReader.Infastructure
{
    public class FeedReaderDb : DbContext, IFeedReaderDataSource
    {
        public DbSet<Feed> Feeds { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        public FeedReaderDb() : base("DefaultConnection")
        {
        }

        IQueryable<Feed> IFeedReaderDataSource.Feeds
        {
            get { return Feeds.OrderBy(f=>f.Name); }
        }

        IQueryable<Subscription> IFeedReaderDataSource.Subscriptions
        {
            get { return Subscriptions; }
        }

        void IFeedReaderDataSource.Save()
        {
            SaveChanges();
        }

        void IFeedReaderDataSource.AddSubscription(Subscription subscription)
        {
            if (Subscriptions.Any(s => s.FeedId == subscription.FeedId && s.UserId == subscription.UserId))
                throw new Exception("Subscription already exits!");

            Subscriptions.Add(subscription);
            SaveChanges();
        }


        void IFeedReaderDataSource.AddFeed(Feed feed)
        {
            if (Feeds.Any(f => f.Name == feed.Name))
                throw new Exception("A Feed by that Name already exists!");

            Feeds.Add(feed);
            SaveChanges();
        }

        void IFeedReaderDataSource.RemoveSubscription(Subscription subscription)
        {
            Subscriptions.Attach(subscription);
            Subscriptions.Remove(subscription);
            SaveChanges();
        }

        XDocument IFeedReaderDataSource.GetFeed(Feed feed)
        {
            return GetFeed(feed);
        }

        private XDocument GetFeed(Feed feed)
        {
            return feed.GetFeed();
        }

        IQueryable<Feed> IFeedReaderDataSource.GetUserFeeds(string userId)
        {
            return GetUserFeeds(userId);
        }

        private IQueryable<Feed> GetUserFeeds(string userId)
        {
            var subs = from s in Subscriptions
                where string.Compare(s.UserId, userId, StringComparison.CurrentCultureIgnoreCase) == 0
                select s.FeedId;

            return from f in Feeds
                where subs.Contains(f.Id)
                orderby (f.Name)
                select f;
        }

        private bool SearchCompare(string source, string value)
        {
            var culture = new CultureInfo("en");
            return (culture.CompareInfo.IndexOf(source, value, CompareOptions.IgnoreCase) >= 0);
        }

        private bool Search(XElement item, string s)
        {
            if (string.IsNullOrEmpty(s))
                return true;

            var tElement = item.Element("title");
            
            if (tElement != null && SearchCompare(tElement.Value, s))
                return true;

            var dElement = item.Element("description");
            if (dElement != null && SearchCompare(dElement.Value, s))
                return true;

            return false;
        }
        
        XDocument IFeedReaderDataSource.GetFeeds(string userId, string s)
        {
            var feedDocs = new List<XDocument>();

            var feeds = GetUserFeeds(userId);

            foreach (var feed in feeds)
                feedDocs.Add(GetFeed(feed));

            var rdoc =
                XDocument.Parse(@"<?xml version='1.0' encoding='utf-8'?><rss version='2.0'><channel></channel></rss>");

            var xElement = rdoc.Element("rss").Element("channel");
            if (xElement != null)
            {
                foreach (var doc in feedDocs)
                    foreach (var item in doc.Descendants("item"))
                    {
                        if (Search(item, s))
                            xElement.Add(item);

                    }

            }
            
            var srtElements = rdoc.Descendants("item")
                .OrderByDescending(p => DateTime.Parse(p.Element("pubDate").Value));

            rdoc = XDocument.Parse(@"<?xml version='1.0' encoding='utf-8'?><rss version='2.0'><channel></channel></rss>");
            xElement = rdoc.Element("rss").Element("channel");
            if (xElement != null)
            {
                foreach (var item in srtElements)
                    xElement.Add(item);
            }

            return rdoc;
        }
    }
}