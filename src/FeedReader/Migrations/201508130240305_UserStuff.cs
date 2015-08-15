namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserStuff : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            AddColumn("dbo.NewsFeeds", "UserProfile_UserId", c => c.Int());
            CreateIndex("dbo.NewsFeeds", "UserProfile_UserId");
            AddForeignKey("dbo.NewsFeeds", "UserProfile_UserId", "dbo.UserProfile", "UserId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NewsFeeds", "UserProfile_UserId", "dbo.UserProfile");
            DropIndex("dbo.NewsFeeds", new[] { "UserProfile_UserId" });
            DropColumn("dbo.NewsFeeds", "UserProfile_UserId");
            DropTable("dbo.UserProfile");
        }
    }
}
