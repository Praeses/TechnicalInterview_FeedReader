namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LotsOChanges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.NewsFeedItems", "NewsFeedModel_NewsFeedID", "dbo.NewsFeedModels");
            DropIndex("dbo.NewsFeedItems", new[] { "NewsFeedModel_NewsFeedID" });
            CreateTable(
                "dbo.NewsFeedItemModels",
                c => new
                    {
                        NewsFeedItemID = c.Int(nullable: false, identity: true),
                        NewsFeedID = c.Int(nullable: false),
                        Title = c.String(nullable: false, maxLength: 144),
                        Description = c.String(nullable: false, maxLength: 1024),
                        DateAdded = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 144),
                    })
                .PrimaryKey(t => t.NewsFeedItemID);
            
            AddColumn("dbo.NewsFeedModels", "SelectedItem", c => c.Int());
            DropTable("dbo.NewsFeedItems");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.NewsFeedItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 144),
                        Description = c.String(nullable: false, maxLength: 1024),
                        DateAdded = c.DateTime(nullable: false),
                        CreatedBy = c.String(maxLength: 144),
                        NewsFeedModel_NewsFeedID = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.NewsFeedModels", "SelectedItem");
            DropTable("dbo.NewsFeedItemModels");
            CreateIndex("dbo.NewsFeedItems", "NewsFeedModel_NewsFeedID");
            AddForeignKey("dbo.NewsFeedItems", "NewsFeedModel_NewsFeedID", "dbo.NewsFeedModels", "NewsFeedID");
        }
    }
}
