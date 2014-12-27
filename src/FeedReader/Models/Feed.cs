using System;
using System.Data.Entity;

namespace FeedReader.Models
{
    public class Feed
    {
        public int ID { get; set; }
        public string siteName { get; set; }
        public string feedAdress { get; set; }
        public int updateInterval { get; set; }
    }

    public class FeedDBContext : DbContext
    {
        public DbSet<Feed> Feeds { get; set; }
    }
}