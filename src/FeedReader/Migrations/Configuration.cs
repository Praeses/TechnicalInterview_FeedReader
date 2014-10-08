using FeedReader.Domain;

namespace FeedReader.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<Infastructure.FeedReaderDb>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(Infastructure.FeedReaderDb context)
        {
            context.Feeds.AddOrUpdate(f => f.Name,
                new Feed { Id = 0, Name = "Scott Hanselman", Url = "http://feeds.hanselman.com/ScottHanselman"},
                new Feed { Id = 0, Name = "Visual Studio Magazine", Url = "http://visualstudiomagazine.com/rss-feeds/blogs.aspx" },
                new Feed { Id = 0, Name = "Yahoo Entertainment", Url = "http://rss.news.yahoo.com/rss/entertainment" },
                new Feed { Id = 0, Name = "Google News", Url = "http://news.google.com/?output=rss" },
                new Feed { Id = 0, Name = "Reuters: Most Read Articles", Url = "http://feeds.reuters.com/reuters/MostRead" },
                new Feed { Id = 0, Name = "NBC News: Science", Url = "http://feeds.nbcnews.com/feeds/science" },
                new Feed { Id = 0, Name = "NPR Arts", Url = "http://www.npr.org/rss/rss.php?id=1008" },
                new Feed { Id = 0, Name = "Fox Sports", Url = "http://feeds.foxnews.com/foxnews/sports" },
                new Feed { Id = 0, Name = "Wired", Url = "http://feeds.wired.com/wired/index" }
            );
        }
    }
}
