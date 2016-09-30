using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FeedReader.Models
{
    public class RssChannel
    {
        public RssChannel()
        {
            Items = new List<RssItem>();
        }

        public int RssChannelId { get; set; }

        public string FeedUrl { get; set; }

        //required fields
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }

        public ICollection<RssItem> Items { get; set; }

        //optional
        public string ImageUrl { get; set; }
        public string LastBuildDate { get; set; }
        public string LastPubDate { get; set; }
        public string Ttl { get; set; }
        public string Language { get; set; }
        public string CopyRight { get; set; }
        public string ManagingEditor { get; set; }

    }

    public class RssItem
    {
        public int RssChannelId {get; set;}

        [ForeignKey("RssChannelId")]
        public RssChannel Channel { get; set; }

        public int RssItemId { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public DateTimeOffset PubDate { get; set; }
        public string ImageUrl { get; set; }
    }

    public class RssSubscription
    {
        public int RssSubscriptionId { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public int RssChannelId { get; set; }
        [ForeignKey("RssChannelId")]
        public RssChannel Feed { get; set; }
    }

    public class RssContext : ApplicationDbContext
    {
        public DbSet<RssChannel> RssChannels { get; set; }
        public DbSet<RssItem> RssItems { get; set; }
        public DbSet<RssSubscription> RssSubscriptions { get; set; }
    }
}