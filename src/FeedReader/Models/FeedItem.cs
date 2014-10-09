using System.Data.Entity;

namespace FeedReader.Models
{
    public class FeedItem
    {
        public string title { get; set; }
        public string address { get; set; }
        public int siteID { get; set; }
    }

    public class FeedItemsDBContext : DbContext
    {
        public DbSet<FeedItem> FeedItems { get; set; }
    }
}