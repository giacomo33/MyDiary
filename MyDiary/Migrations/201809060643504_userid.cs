namespace MyDiary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Entry", "UserId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Entry", "UserId");
        }
    }
}
