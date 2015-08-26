namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Profile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NewsFeeds", "ApplicationUser_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.NewsFeeds", "ApplicationUser_Id");
            AddForeignKey("dbo.NewsFeeds", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NewsFeeds", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.NewsFeeds", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.NewsFeeds", "ApplicationUser_Id");
        }
    }
}
