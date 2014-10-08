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
            context.Feeds.AddOrUpdate(d => d.Name,
                new Feed { Name = "Test", Url = "test1"},
                new Feed { Name = "FooBar", Url = "foo"}
            );
        }
    }
}
