namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Bye : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApplicationUserNewsFeeds",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        NewsFeed_NewsFeedID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.NewsFeed_NewsFeedID })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .ForeignKey("dbo.NewsFeeds", t => t.NewsFeed_NewsFeedID, cascadeDelete: true)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.NewsFeed_NewsFeedID);
            
            DropColumn("dbo.AspNetUsers", "Hmm");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Hmm", c => c.String());
            DropForeignKey("dbo.ApplicationUserNewsFeeds", "NewsFeed_NewsFeedID", "dbo.NewsFeeds");
            DropForeignKey("dbo.ApplicationUserNewsFeeds", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.ApplicationUserNewsFeeds", new[] { "NewsFeed_NewsFeedID" });
            DropIndex("dbo.ApplicationUserNewsFeeds", new[] { "ApplicationUser_Id" });
            DropTable("dbo.ApplicationUserNewsFeeds");
        }
    }
}
