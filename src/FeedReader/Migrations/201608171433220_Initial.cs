namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Feeds",
                c => new
                {
                    FeedId = c.Int(nullable: false, identity: true),
                    UserId = c.String(nullable: false, name: "Id", maxLength: 128),
                    Image = c.String(),
                    Title = c.String(),
                    URL = c.String(),
                })
                .PrimaryKey(f => f.FeedId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: false)
                .Index(t => t.FeedId);

            CreateTable(
                "dbo.FeedItems",
                c => new
                {
                    FeedItemId = c.Int(nullable: false, identity: true),
                    FeedId = c.Int(nullable: false),
                    Image = c.String(),
                    Description = c.String(),
                    Title = c.String(),
                    PublishedDate = c.DateTime(),
                    URL = c.String(),
                })
                .PrimaryKey(t => t.FeedItemId)
                .ForeignKey("dbo.Feeds", t => t.FeedId, cascadeDelete: true)
                .Index(t => t.FeedId);
        }

        public override void Down()
        {
            DropForeignKey("dbo.FeedItems", "FeedId", "dbo.Feeds");
            DropIndex("dbo.FeedItems", new[] { "FeedId" });
            DropTable("dbo.FeedItems");
            DropTable("dbo.Feeds");
        }
    }
}
