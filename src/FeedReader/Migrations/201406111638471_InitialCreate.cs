namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserFeed",
                c => new
                    {
                        UserFeedID = c.Int(nullable: false, identity: true),
                        UserID = c.Guid(nullable: false),
                        Url = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.UserFeedID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserFeed");
        }
    }
}
