namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateUserFeedModel : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserFeed", "UserID", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserFeed", "UserID", c => c.String(nullable: false));
        }
    }
}
