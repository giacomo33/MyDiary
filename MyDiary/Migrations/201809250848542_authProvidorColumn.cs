namespace MyDiary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class authProvidorColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "AuthenticationProvidor", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "AuthenticationProvidor");
        }
    }
}
