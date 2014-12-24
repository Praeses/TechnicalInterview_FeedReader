namespace FeedReader.Migrations
{
    using FeedReader.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FeedReader.Models.UserSubscriptionDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(FeedReader.Models.UserSubscriptionDBContext context)
        {
            context.UserSubscriptions.AddOrUpdate(i => new { i.rssFeedURL , i.userName},//NOTE: This database has two primary keys: rssFeedURL and userName
                new UserSubscription
                {
                    userName = "test@test.test",
                    rssFeedURL = "http://rss.nytimes.com/services/xml/rss/nyt/Technology.xml",
                    rssFeedName = "NYT_Tech"
                });
            context.UserSubscriptions.AddOrUpdate(i => new { i.rssFeedURL, i.userName },//NOTE: This database has two primary keys: rssFeedURL and userName
                new UserSubscription
                {
                    userName = "test@test.test",
                    rssFeedURL = "http://www.npr.org/rss/rss.php?id=2",
                    rssFeedName = "All Things Considered"
                });
            context.UserSubscriptions.AddOrUpdate(i => new { i.rssFeedURL, i.userName },//NOTE: This database has two primary keys: rssFeedURL and userName
               new UserSubscription
               {
                   userName = "test@test.test",
                   rssFeedURL = "http://www.npr.org/rss/rss.php?id=1019",
                   rssFeedName = "NPR Tech"
               });
            context.UserSubscriptions.AddOrUpdate(i => new { i.rssFeedURL, i.userName },//NOTE: This database has two primary keys: rssFeedURL and userName
               new UserSubscription
               {
                   userName = "test@test.test",
                   rssFeedURL = "http://www.npr.org/rss/rss.php?id=1049",
                   rssFeedName = "NPR Digital Life"
               });
        }
    }
}
