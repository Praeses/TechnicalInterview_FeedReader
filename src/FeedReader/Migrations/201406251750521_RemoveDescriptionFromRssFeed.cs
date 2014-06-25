namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveDescriptionFromRssFeed : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.RssFeeds", "Description");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RssFeeds", "Description", c => c.String());
        }
    }
}
