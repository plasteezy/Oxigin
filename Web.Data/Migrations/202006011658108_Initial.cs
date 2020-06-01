namespace Web.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CategoryDataSets",
                c => new
                    {
                        CategoryDataSetId = c.Int(nullable: false, identity: true),
                        ProjectDataSetId = c.Int(nullable: false),
                        Label = c.String(),
                        Score = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.CategoryDataSetId)
                .ForeignKey("dbo.ProjectDataSets", t => t.ProjectDataSetId, cascadeDelete: true)
                .Index(t => t.ProjectDataSetId);
            
            CreateTable(
                "dbo.ProjectDataSets",
                c => new
                    {
                        ProjectDataSetId = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        Entry = c.String(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectDataSetId)
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .Index(t => t.ProjectId);
            
            CreateTable(
                "dbo.EmotionDataSets",
                c => new
                    {
                        EmotionDataSetId = c.Int(nullable: false, identity: true),
                        ProjectDataSetId = c.Int(nullable: false),
                        AngerScore = c.Double(nullable: false),
                        JoyScore = c.Double(nullable: false),
                        DisgustScore = c.Double(nullable: false),
                        SadnessScore = c.Double(nullable: false),
                        FearScore = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.EmotionDataSetId)
                .ForeignKey("dbo.ProjectDataSets", t => t.ProjectDataSetId, cascadeDelete: true)
                .Index(t => t.ProjectDataSetId);
            
            CreateTable(
                "dbo.KeywordDataSets",
                c => new
                    {
                        KeywordDataSetId = c.Int(nullable: false, identity: true),
                        ProjectDataSetId = c.Int(nullable: false),
                        Text = c.String(),
                        Relevance = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.KeywordDataSetId)
                .ForeignKey("dbo.ProjectDataSets", t => t.ProjectDataSetId, cascadeDelete: true)
                .Index(t => t.ProjectDataSetId);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        ProjectId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectId);
            
            CreateTable(
                "dbo.SentimentDataSets",
                c => new
                    {
                        SentimentDataSetId = c.Int(nullable: false, identity: true),
                        ProjectDataSetId = c.Int(nullable: false),
                        SentimentLabel = c.String(),
                        SentimentScore = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.SentimentDataSetId)
                .ForeignKey("dbo.ProjectDataSets", t => t.ProjectDataSetId, cascadeDelete: true)
                .Index(t => t.ProjectDataSetId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.SavedQueries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Definition = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        ConnectionString = c.String(),
                        Date = c.DateTime(nullable: false),
                        Config = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        LastLoginDate = c.DateTime(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.SentimentDataSets", "ProjectDataSetId", "dbo.ProjectDataSets");
            DropForeignKey("dbo.ProjectDataSets", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.KeywordDataSets", "ProjectDataSetId", "dbo.ProjectDataSets");
            DropForeignKey("dbo.EmotionDataSets", "ProjectDataSetId", "dbo.ProjectDataSets");
            DropForeignKey("dbo.CategoryDataSets", "ProjectDataSetId", "dbo.ProjectDataSets");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.SentimentDataSets", new[] { "ProjectDataSetId" });
            DropIndex("dbo.KeywordDataSets", new[] { "ProjectDataSetId" });
            DropIndex("dbo.EmotionDataSets", new[] { "ProjectDataSetId" });
            DropIndex("dbo.ProjectDataSets", new[] { "ProjectId" });
            DropIndex("dbo.CategoryDataSets", new[] { "ProjectDataSetId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.SavedQueries");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.SentimentDataSets");
            DropTable("dbo.Projects");
            DropTable("dbo.KeywordDataSets");
            DropTable("dbo.EmotionDataSets");
            DropTable("dbo.ProjectDataSets");
            DropTable("dbo.CategoryDataSets");
        }
    }
}
