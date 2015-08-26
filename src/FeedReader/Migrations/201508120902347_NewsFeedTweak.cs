namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewsFeedTweak : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.NewsFeedItemModels", "NewsFeedID");
            AddForeignKey("dbo.NewsFeedItemModels", "NewsFeedID", "dbo.NewsFeedModels", "NewsFeedID", cascadeDelete: true);
            DropColumn("dbo.NewsFeedModels", "SelectedItem");
        }
        
        public override void Down()
        {
            AddColumn("dbo.NewsFeedModels", "SelectedItem", c => c.Int());
            DropForeignKey("dbo.NewsFeedItemModels", "NewsFeedID", "dbo.NewsFeedModels");
            DropIndex("dbo.NewsFeedItemModels", new[] { "NewsFeedID" });
        }
    }
}
