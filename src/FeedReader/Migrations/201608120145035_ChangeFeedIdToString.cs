namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeFeedIdToString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Feeds", "UserId", c => c.String(nullable: false));
            AlterColumn("dbo.Feeds", "URL", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Feeds", "URL", c => c.String());
            AlterColumn("dbo.Feeds", "UserId", c => c.Int(nullable: false));
        }
    }
}
