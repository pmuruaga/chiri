namespace ControlBingosChiri.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NumerosSorteos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NumVal = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            //DropTable("dbo.UserProfile");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            DropTable("dbo.NumerosSorteos");
        }
    }
}
