namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Oops : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserProfile", "What", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserProfile", "What", c => c.Int(nullable: false));
        }
    }
}
