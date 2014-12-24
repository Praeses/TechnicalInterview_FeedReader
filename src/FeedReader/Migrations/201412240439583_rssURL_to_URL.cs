namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rssURL_to_URL : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserSubscriptions", "rssFeedURL", c => c.String(maxLength: 300));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserSubscriptions", "rssFeedURL");
        }
    }
}
