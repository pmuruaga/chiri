namespace ControlBingosChiri.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopupTextoAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PopupTextoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ShowPopup = c.Boolean(nullable: false),
                        Titulo = c.String(),
                        Contenido = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.NumerosSorteos", "FechaSorteo", c => c.DateTime(nullable: false));
            AddColumn("dbo.NumerosSorteos", "FechaCreacion", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.NumerosSorteos", "FechaCreacion");
            DropColumn("dbo.NumerosSorteos", "FechaSorteo");
            DropTable("dbo.PopupTextoes");
        }
    }
}
