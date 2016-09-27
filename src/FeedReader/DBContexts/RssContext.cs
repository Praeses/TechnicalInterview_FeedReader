using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using FeedReader.Models;

namespace FeedReader.DBContexts
{
    public class RssContext : DbContext
    {
        public DbSet<RssChannel> RssChannels { get; set; }
    }
}