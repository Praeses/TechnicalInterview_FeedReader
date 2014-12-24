using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace FeedReader.Models
{
    public class UserSubscription
    {
        public int ID { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 3)]
        [Display(Name="User")]
        public string userName { get; set; }

        [Required]
        [Url]
        [StringLength(300, MinimumLength = 5)]
        [Display(Name = "RSS Feed")]
        public string rssFeedURL { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 3)]
        [Display(Name = "RSS Name")]
        public string rssFeedName { get; set; }
    }

    public class UserSubscriptionDBContext:DbContext
    {
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
    }
}