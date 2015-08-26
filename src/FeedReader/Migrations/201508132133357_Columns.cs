namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Columns : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserProfile", "UserName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserProfile", "UserName", c => c.String());
        }
    }
}
