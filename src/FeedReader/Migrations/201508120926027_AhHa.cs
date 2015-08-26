namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AhHa : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.NewsFeedItemModels", newName: "NewsFeedItems");
            RenameTable(name: "dbo.NewsFeedModels", newName: "NewsFeeds");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.NewsFeeds", newName: "NewsFeedModels");
            RenameTable(name: "dbo.NewsFeedItems", newName: "NewsFeedItemModels");
        }
    }
}
