Imports System
Imports System.Data.Entity.Migrations
Imports Microsoft.VisualBasic

Namespace Migrations
    Public Partial Class Initial
        Inherits DbMigration
    
        Public Overrides Sub Up()
            CreateTable(
                "dbo.AspNetRoles",
                Function(c) New With
                    {
                        .Id = c.String(nullable := False, maxLength := 128),
                        .Name = c.String(nullable := False)
                    }) _
                .PrimaryKey(Function(t) t.Id)
            
            CreateTable(
                "dbo.AspNetUsers",
                Function(c) New With
                    {
                        .Id = c.String(nullable := False, maxLength := 128),
                        .UserName = c.String(),
                        .PasswordHash = c.String(),
                        .SecurityStamp = c.String(),
                        .Discriminator = c.String(nullable := False, maxLength := 128)
                    }) _
                .PrimaryKey(Function(t) t.Id)
            
            CreateTable(
                "dbo.AspNetUserClaims",
                Function(c) New With
                    {
                        .Id = c.Int(nullable := False, identity := True),
                        .ClaimType = c.String(),
                        .ClaimValue = c.String(),
                        .User_Id = c.String(nullable := False, maxLength := 128)
                    }) _
                .PrimaryKey(Function(t) t.Id) _
                .ForeignKey("dbo.AspNetUsers", Function(t) t.User_Id, cascadeDelete := True) _
                .Index(Function(t) t.User_Id)
            
            CreateTable(
                "dbo.AspNetUserLogins",
                Function(c) New With
                    {
                        .UserId = c.String(nullable := False, maxLength := 128),
                        .LoginProvider = c.String(nullable := False, maxLength := 128),
                        .ProviderKey = c.String(nullable := False, maxLength := 128)
                    }) _
                .PrimaryKey(Function(t) New With { t.UserId, t.LoginProvider, t.ProviderKey }) _
                .ForeignKey("dbo.AspNetUsers", Function(t) t.UserId, cascadeDelete := True) _
                .Index(Function(t) t.UserId)
            
            CreateTable(
                "dbo.AspNetUserRoles",
                Function(c) New With
                    {
                        .UserId = c.String(nullable := False, maxLength := 128),
                        .RoleId = c.String(nullable := False, maxLength := 128)
                    }) _
                .PrimaryKey(Function(t) New With { t.UserId, t.RoleId }) _
                .ForeignKey("dbo.AspNetRoles", Function(t) t.RoleId, cascadeDelete := True) _
                .ForeignKey("dbo.AspNetUsers", Function(t) t.UserId, cascadeDelete := True) _
                .Index(Function(t) t.RoleId) _
                .Index(Function(t) t.UserId)
            
        End Sub
        
        Public Overrides Sub Down()
            DropForeignKey("dbo.AspNetUserClaims", "User_Id", "dbo.AspNetUsers")
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers")
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles")
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers")
            DropIndex("dbo.AspNetUserClaims", New String() { "User_Id" })
            DropIndex("dbo.AspNetUserRoles", New String() { "UserId" })
            DropIndex("dbo.AspNetUserRoles", New String() { "RoleId" })
            DropIndex("dbo.AspNetUserLogins", New String() { "UserId" })
            DropTable("dbo.AspNetUserRoles")
            DropTable("dbo.AspNetUserLogins")
            DropTable("dbo.AspNetUserClaims")
            DropTable("dbo.AspNetUsers")
            DropTable("dbo.AspNetRoles")
        End Sub
    End Class
End Namespace
