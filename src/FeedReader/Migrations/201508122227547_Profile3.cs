namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Profile3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.NewsFeeds", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.NewsFeeds", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.NewsFeeds", "ApplicationUser_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.NewsFeeds", "ApplicationUser_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.NewsFeeds", "ApplicationUser_Id");
            AddForeignKey("dbo.NewsFeeds", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
