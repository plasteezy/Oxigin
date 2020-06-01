namespace Web.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddApiCredentials : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApiCredentials",
                c => new
                    {
                        ApiCredentialId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        Key = c.String(),
                        Secret = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ApiCredentialId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ApiCredentials");
        }
    }
}
