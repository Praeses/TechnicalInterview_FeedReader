namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Alias : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Alias", c => c.String());
            DropColumn("dbo.AspNetUsers", "Initials");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Initials", c => c.String());
            DropColumn("dbo.AspNetUsers", "Alias");
        }
    }
}
