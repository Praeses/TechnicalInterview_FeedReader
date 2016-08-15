namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTitleToFeeds : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Feeds", "Title", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Feeds", "Title");
        }
    }
}
