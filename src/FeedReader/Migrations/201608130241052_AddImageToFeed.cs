namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddImageToFeed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Feeds", "Image", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Feeds", "Image");
        }
    }
}
