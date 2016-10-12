using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;


using FeedReader.Utils;

namespace FeedReader.Models
{
    /// <summary>
    /// Model object presentating the database structure of the "RssChannel" element.
    /// A Hash column has been added as the unique item FeedUrl is too long to be used as a key.
    /// </summary>
    public class RssChannel
    {
        public RssChannel()
        {
            Items = new List<RssItem>();
        }

        public int RssChannelId { get; set; }
        public string FeedUrl { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } //handle locking for rss channel updates

        [Index("IX_RssChanHash", 1, IsUnique = true)]
        [MaxLength(40)]
        public string Hash
        {
            get
            {
                //basic hashing on title and pub date to determine uniqueness. This would be better suited later as a check in the database itself rather than code
                return FeedReaderUtils.GetHashString(FeedUrl != null ? FeedUrl : String.Empty);
            }
            set
            {

            }
        }

        //required fields
        //bug with title being null. Need to handle setting the title to the feed url if it is not present
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

    /// <summary>
    /// Object representation of an RssFeedItem. 
    /// An interna Hash Property is used as the unqiue identifiers consist of a string whose length makes it too long to be used as a key
    /// </summary>
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

        [Index("IX_RssHash", 1, IsUnique = true)]
        [MaxLength(40)]
        public string Hash
        {
            get
            {
                //basic hashing on title and pub date to determine uniqueness. This would be better suited later as a check in the database itself rather than code
                return FeedReaderUtils.GetHashString(RssChannelId + Title + PubDate + "");
            }
            set
            {

            }
        }
        /*public UserRssAttributes UserAttributes { get; set; } */
    }

    /// <summary>
    /// Object representation of attributes that apply to an RssItem for a particiular user. This is to keep the channels/items which do not change separate from the user
    /// with a subscription object tieing them together. This allows customer attributes to be added with duplicate rows for rssChannels and rssItems
    /// </summary>
    public class UserRssAttributes
    {
        public int UserRssAttributesId { get; set; }
        public int RssItemId { get; set; }

        [ForeignKey("RssItemId")]
        public RssItem RssItem { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public bool Read { get; set; }
        public bool Hidden { get; set; }
    }

    /// <summary>
    /// Object representation of a user's subscription to an rss feed. 
    /// Prevents coupling of an RssChannel to a particular user allowing them to be reused across all user's that request them
    /// </summary>
    public class RssSubscription
    {
        public int RssSubscriptionId { get; set; }

        [Index("IX_RssSub", 1, IsUnique = true)]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Index("IX_RssSub", 2, IsUnique = true)]
        public int RssChannelId { get; set; }
        [ForeignKey("RssChannelId")]
        public RssChannel Feed { get; set; }
    }

    /// <summary>
    /// Application database context that holds the elements related to the RSS functionality of the application. Extends the existing ApplicationDbContext so the items 
    /// will be present in the same database
    /// </summary>
    public class RssContext : ApplicationDbContext
    {
        public DbSet<RssChannel> RssChannels { get; set; }
        public DbSet<RssItem> RssItems { get; set; }
        public DbSet<RssSubscription> RssSubscriptions { get; set; }
        public DbSet<UserRssAttributes> UserRssAttributes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RssChannel>().Property(p => p.RowVersion).IsConcurrencyToken();

            base.OnModelCreating(modelBuilder);
        }
    }


}