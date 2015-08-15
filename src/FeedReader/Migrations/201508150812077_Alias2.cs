namespace FeedReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Alias2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "Alias", c => c.String(maxLength: 6));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "Alias", c => c.String());
        }
    }
}
