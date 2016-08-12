namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeUserIdNotRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Feeds", "UserId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Feeds", "UserId", c => c.String(nullable: false));
        }
    }
}
