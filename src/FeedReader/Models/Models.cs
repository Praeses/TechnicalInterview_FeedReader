using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FeedReader.Models
{
    // Class for rss feed
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

        // Holds reference to rss feed item children
        public List<RssFeedItem> RssFeedItems { get; set; }
    }

    // Class for rss feed item
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
        public string ImageUrl { get; set; }

        // Holds reference to owning rss feed
        public RssFeed RssFeed { get; set; }
    }

    // Class for a user rss feed, connects user to their rss feed
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

        // Reference to rss feed
        public RssFeed RssFeed { get; set; }       
        // Reference to user
        public ApplicationUser ApplicationUser { get; set; }
    }
}