namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedItems : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MyFeedItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 144),
                        Description = c.String(nullable: false, maxLength: 1024),
                        DateAdded = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        NewsFeedModel_NewsFeedID = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.NewsFeedModels", t => t.NewsFeedModel_NewsFeedID)
                .Index(t => t.NewsFeedModel_NewsFeedID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MyFeedItems", "NewsFeedModel_NewsFeedID", "dbo.NewsFeedModels");
            DropIndex("dbo.MyFeedItems", new[] { "NewsFeedModel_NewsFeedID" });
            DropTable("dbo.MyFeedItems");
        }
    }
}
