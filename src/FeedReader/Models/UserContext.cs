using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FeedReader.Models
{
    public class UserContext : DbContext
    {
        public DbSet<AspNetUserInfo> AspNetUserInfo { get; set; }
        public DbSet<AspNetFeed> AspNetFeed { get; set; }
        public DbSet<AspNetUserFeed> AspNetUserFeed { get; set; }
    }
}