using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FeedReader.Models
{
    public class RssFeed
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RssFeedId { get; set; }

        [Index(IsUnique=true)]
        [Column(TypeName = "VARCHAR")]
        [StringLength(450)]
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public List<RssFeedItem> RssFeedItems { get; set; }
    }

    public class RssFeedItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("RssFeed")]
        public int RssFeedId { get; set; }

        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public DateTime PublishDate { get; set; }
        public string PublishDateString { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }

        public RssFeed RssFeed { get; set; }
    }

    public class UserRssFeed
    {
        [Key]
        [ForeignKey("RssFeed")]
        [Column(Order = 1)]
        public int RssFeedId { get; set; }

        [Key]
        [ForeignKey("ApplicationUser")]
        [Column(Order = 2)]
        public string UserId { get; set; }

        public RssFeed RssFeed { get; set; }       
        public ApplicationUser ApplicationUser { get; set; }
    }

    public class FeedReaderContext : ApplicationDbContext
    {
        public DbSet<UserRssFeed> UserRssFeeds { get; set; }
        public DbSet<RssFeed> RssFeeds { get; set; }
        public DbSet<RssFeedItem> RssFeedItems { get; set; }
    }
}