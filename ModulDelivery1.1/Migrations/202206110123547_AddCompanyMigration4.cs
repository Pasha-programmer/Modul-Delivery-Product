namespace ModulDelivery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCompanyMigration4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courier", "AddressId", c => c.Int());
            CreateIndex("dbo.Courier", "AddressId");
            AddForeignKey("dbo.Courier", "AddressId", "dbo.Address", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Courier", "AddressId", "dbo.Address");
            DropIndex("dbo.Courier", new[] { "AddressId" });
            DropColumn("dbo.Courier", "AddressId");
        }
    }
}
