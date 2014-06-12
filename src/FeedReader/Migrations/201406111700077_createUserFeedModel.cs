namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createUserFeedModel : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserFeed", "UserID", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserFeed", "UserID", c => c.Guid(nullable: false));
        }
    }
}
