namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedInitialRoutes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NewsFeedModel",
                c => new
                    {
                        NewsFeedID = c.Int(nullable: false, identity: true),
                        Category = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.NewsFeedID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.NewsFeedModel");
        }
    }
}
