namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201505150512548_add_feed_model : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Feeds", "channel", c => c.String(nullable: false));
            AlterColumn("dbo.Feeds", "link", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Feeds", "link", c => c.String());
            AlterColumn("dbo.Feeds", "channel", c => c.String());
        }
    }
}
