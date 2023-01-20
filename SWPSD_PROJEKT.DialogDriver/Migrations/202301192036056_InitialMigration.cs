namespace SWPSD_PROJEKT.DialogDriver.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Facilities",
                c => new
                    {
                        FacilityId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.FacilityId);
            
            CreateTable(
                "dbo.FacilityOrders",
                c => new
                    {
                        FacilityOrderId = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        FacilityId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FacilityOrderId)
                .ForeignKey("dbo.Facilities", t => t.FacilityId, cascadeDelete: true)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.OrderId)
                .Index(t => t.FacilityId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderId = c.Int(nullable: false, identity: true),
                        RoomId = c.Int(nullable: false),
                        GuestId = c.Int(nullable: false),
                        FromDate = c.DateTime(nullable: false),
                        ToDate = c.DateTime(nullable: false),
                        NumberOfPeople = c.Int(nullable: false),
                        Days = c.Int(nullable: false),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.OrderId)
                .ForeignKey("dbo.Guests", t => t.GuestId, cascadeDelete: true)
                .ForeignKey("dbo.Rooms", t => t.RoomId, cascadeDelete: true)
                .Index(t => t.RoomId)
                .Index(t => t.GuestId);
            
            CreateTable(
                "dbo.Guests",
                c => new
                    {
                        GuestId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Surname = c.String(),
                        BirthYear = c.Int(nullable: false),
                        Email = c.String(),
                        PhoneNumber = c.String(),
                        CreditCardNumber = c.String(),
                    })
                .PrimaryKey(t => t.GuestId);
            
            CreateTable(
                "dbo.Rooms",
                c => new
                    {
                        RoomId = c.Int(nullable: false, identity: true),
                        RoomTypeId = c.Int(nullable: false),
                        Capacity = c.Int(nullable: false),
                        PricePerNight = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.RoomId)
                .ForeignKey("dbo.RoomTypes", t => t.RoomTypeId, cascadeDelete: true)
                .Index(t => t.RoomTypeId);
            
            CreateTable(
                "dbo.RoomTypes",
                c => new
                    {
                        RoomTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.RoomTypeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FacilityOrders", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Orders", "RoomId", "dbo.Rooms");
            DropForeignKey("dbo.Rooms", "RoomTypeId", "dbo.RoomTypes");
            DropForeignKey("dbo.Orders", "GuestId", "dbo.Guests");
            DropForeignKey("dbo.FacilityOrders", "FacilityId", "dbo.Facilities");
            DropIndex("dbo.Rooms", new[] { "RoomTypeId" });
            DropIndex("dbo.Orders", new[] { "GuestId" });
            DropIndex("dbo.Orders", new[] { "RoomId" });
            DropIndex("dbo.FacilityOrders", new[] { "FacilityId" });
            DropIndex("dbo.FacilityOrders", new[] { "OrderId" });
            DropTable("dbo.RoomTypes");
            DropTable("dbo.Rooms");
            DropTable("dbo.Guests");
            DropTable("dbo.Orders");
            DropTable("dbo.FacilityOrders");
            DropTable("dbo.Facilities");
        }
    }
}
