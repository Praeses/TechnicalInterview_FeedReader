using System.Data.Entity;

namespace BozemansFeedReader.Models
{
    public class Feed
    {
        public int ID { get; set; }
        public string FeedTitle { get; set; }
        public string FeedURL { get; set; }
    }

    public class FeedDBContext : DbContext
    {
        public DbSet<Feed> Feeds { get; set; }
    }
}