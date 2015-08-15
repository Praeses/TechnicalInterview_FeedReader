namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MaybeNow : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.NewsFeeds", "UserProfile_UserId", "dbo.UserProfile");
            DropIndex("dbo.NewsFeeds", new[] { "UserProfile_UserId" });
            AddColumn("dbo.AspNetUsers", "Hmm", c => c.String());
            DropColumn("dbo.NewsFeeds", "UserProfile_UserId");
            DropTable("dbo.UserProfile");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false),
                        What = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            AddColumn("dbo.NewsFeeds", "UserProfile_UserId", c => c.Int());
            DropColumn("dbo.AspNetUsers", "Hmm");
            CreateIndex("dbo.NewsFeeds", "UserProfile_UserId");
            AddForeignKey("dbo.NewsFeeds", "UserProfile_UserId", "dbo.UserProfile", "UserId");
        }
    }
}
