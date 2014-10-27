using FeedReader.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;

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

        IEnumerable<FeedItem> IFeedReaderDataSource.GetFeed(Feed feed)
        {
            return GetFeed(feed);
        }

        private IEnumerable<FeedItem> GetFeed(Feed feed)
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

        private bool Search(FeedItem item, string s)
        {
            if (string.IsNullOrEmpty(s))
                return true;

            if( SearchCompare(item.Title, s))
                return true;

            if (SearchCompare(item.Description, s))
                return true;

            return false;
        }

        IEnumerable<FeedItem> IFeedReaderDataSource.GetFeeds(string userId, string s)
        {
            var feedList = new List<FeedItem>();
            var feeds = GetUserFeeds(userId);

            foreach (var feed in feeds)
            {
                feedList.AddRange(feed.GetFeed().Where(item => Search(item, s)));
            }

            return feedList.OrderByDescending(p => p.PublishDate);
        }
    }
}