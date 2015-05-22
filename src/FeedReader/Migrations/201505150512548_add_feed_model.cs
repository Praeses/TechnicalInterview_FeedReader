namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_feed_model : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Feeds",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        user_id = c.String(),
                        channel = c.String(nullable: false),
                        link = c.String(nullable: false),
                        desc = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Feeds");
        }
    }
}
