namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTitleToUserFeedModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserFeed", "Title", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserFeed", "Title");
        }
    }
}
