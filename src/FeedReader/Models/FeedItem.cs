using System.Data.Entity;

namespace FeedReader.Models
{
    public class FeedItem
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string address { get; set; }
    }

    public class FeedItemsDBContext : DbContext
    {
        public DbSet<FeedItem> FeedItems { get; set; }
    }
}