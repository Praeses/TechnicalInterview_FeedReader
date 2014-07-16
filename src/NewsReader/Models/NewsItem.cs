using System;
using System.Data.Entity;

namespace NewsReader.Models
{
    public class NewsItem
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Links { get; set; }
        public DateTime PublishDate { get; set; }
        public string Summary { get; set; }
    }

    public class NewsDBContext : DbContext
    {
        public DbSet<NewsItem> NewsDB { get; set; }
    }
}