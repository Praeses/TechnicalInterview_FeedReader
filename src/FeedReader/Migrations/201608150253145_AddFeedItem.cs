namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFeedItem : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FeedItems",
                c => new
                    {
                        FeedItemId = c.Int(nullable: false, identity: true),
                        FeedId = c.Int(nullable: false),
                        Image = c.String(),
                        Description = c.String(),
                        Title = c.String(),
                        PublishedDate = c.DateTime(),
                        URL = c.String(),
                    })
                .PrimaryKey(t => t.FeedItemId)
                .ForeignKey("dbo.Feeds", t => t.FeedId, cascadeDelete: true)
                .Index(t => t.FeedId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FeedItems", "FeedId", "dbo.Feeds");
            DropIndex("dbo.FeedItems", new[] { "FeedId" });
            DropTable("dbo.FeedItems");
        }
    }
}
