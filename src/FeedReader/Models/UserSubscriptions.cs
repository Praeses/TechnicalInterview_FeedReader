using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace FeedReader.Models
{
    public class UserSubscription
    {
        public int ID { get; set; }
        public string userName { get; set; }
        public string rssFeedURL { get; set; }

    }

    public class UserSubscriptionDBContext:DbContext
    {
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
    }
}