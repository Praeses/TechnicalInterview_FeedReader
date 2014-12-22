namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rssFeedName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserSubscriptions", "rssFeedName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserSubscriptions", "rssFeedName");
        }
    }
}
