namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class required_validation_userSubscriptions : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserSubscriptions", "userName", c => c.String(nullable: false, maxLength: 60));
            AlterColumn("dbo.UserSubscriptions", "rssFeedURL", c => c.String(nullable: false, maxLength: 300));
            AlterColumn("dbo.UserSubscriptions", "rssFeedName", c => c.String(nullable: false, maxLength: 60));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserSubscriptions", "rssFeedName", c => c.String(maxLength: 60));
            AlterColumn("dbo.UserSubscriptions", "rssFeedURL", c => c.String(maxLength: 300));
            AlterColumn("dbo.UserSubscriptions", "userName", c => c.String(maxLength: 60));
        }
    }
}
