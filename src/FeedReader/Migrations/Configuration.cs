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
                    userName = "testName",
                    rssFeedURL = "testFeedURL",
                    rssFeedName = "testFeedName"
                });
        }
    }
}
