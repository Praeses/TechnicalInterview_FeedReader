namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedName : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.MyFeedItems", newName: "NewsFeedItems");
            AlterColumn("dbo.NewsFeedItems", "CreatedBy", c => c.String(maxLength: 144));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.NewsFeedItems", "CreatedBy", c => c.String());
            RenameTable(name: "dbo.NewsFeedItems", newName: "MyFeedItems");
        }
    }
}
