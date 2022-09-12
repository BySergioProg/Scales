namespace Scales.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cars",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CarType = c.String(),
                        CarNo = c.String(),
                        CarWeight = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Nomenclatures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NomenclatureName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Shipments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ShipDateTime = c.DateTime(nullable: false),
                        CarType = c.String(),
                        CarNo = c.String(),
                        NomenclatureName = c.String(),
                        CarWeight = c.Double(nullable: false),
                        Brutto = c.Double(nullable: false),
                        Netto = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.CarTypes", "Type", c => c.String());
            DropColumn("dbo.CarTypes", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CarTypes", "Name", c => c.String());
            DropColumn("dbo.CarTypes", "Type");
            DropTable("dbo.Shipments");
            DropTable("dbo.Nomenclatures");
            DropTable("dbo.Cars");
        }
    }
}
