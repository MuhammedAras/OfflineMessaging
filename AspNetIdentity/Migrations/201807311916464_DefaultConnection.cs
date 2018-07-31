namespace AspNetIdentity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DefaultConnection : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "FirstName", c => c.String(nullable: false, maxLength: 100));
            DropColumn("dbo.AspNetUsers", "FirsName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "FirsName", c => c.String(nullable: false, maxLength: 100));
            DropColumn("dbo.AspNetUsers", "FirstName");
        }
    }
}
