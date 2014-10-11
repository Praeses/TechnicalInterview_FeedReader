using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FeedReader.Models
{
    public class Feed
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
    }

    public class FeedItem
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public DateTime PublishDate { get; set; }

        public int FeedId { get; set; }
        public virtual Feed Feed { get; set; }
    }

    public class UserFeed
    {
        public int ID { get; set; }

        public string UserId { get; set; }

        public int FeedId { get; set; }

        public virtual Feed Feed { get; set; }
    }

    public class FeedDBContext : DbContext
    {
        public DbSet<Feed> Feeds { get; set; }
        public DbSet<FeedItem> FeedItems { get; set; }
        public DbSet<UserFeed> UserFeeds { get; set; }
    }
}