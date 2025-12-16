using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TicketMaster.Migrations
{
    /// <inheritdoc />
    public partial class InitialRolePermissionSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ColorCode = table.Column<string>(type: "TEXT", nullable: false),
                    FrontSize = table.Column<string>(type: "TEXT", nullable: false),
                    FontFamily = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    EventDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    MaxTickets = table.Column<int>(type: "INTEGER", nullable: false),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedTicketsCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvitationsWithPoint",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Issued = table.Column<string>(type: "TEXT", nullable: false),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: false),
                    CoordinateX = table.Column<int>(type: "INTEGER", nullable: false),
                    CoordinateY = table.Column<int>(type: "INTEGER", nullable: false),
                    InvitationType = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvitationsWithPoint", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Issueds",
                columns: table => new
                {
                    UserCode = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Issueds", x => x.UserCode);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: true),
                    LastName = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Invitations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InviterName = table.Column<string>(type: "TEXT", nullable: false),
                    InvitationType = table.Column<string>(type: "TEXT", nullable: false),
                    Issued = table.Column<string>(type: "TEXT", nullable: false),
                    UniqCode = table.Column<string>(type: "TEXT", nullable: false),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EventId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invitations_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Invitations_Issueds_Issued",
                        column: x => x.Issued,
                        principalTable: "Issueds",
                        principalColumn: "UserCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<int>(type: "INTEGER", nullable: false),
                    PermissionId = table.Column<int>(type: "INTEGER", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    RoleId = table.Column<int>(type: "INTEGER", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Category", "Code", "CreatedAt", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Tickets", "tickets.manage", new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Create, view, edit, and delete tickets", "Manage Tickets" },
                    { 2, "Tickets", "tickets.view", new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), "View tickets only", "View Tickets" },
                    { 3, "Events", "events.manage", new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Create, view, edit, and delete events", "Manage Events" },
                    { 4, "Events", "events.view", new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), "View events only", "View Events" },
                    { 5, "Users", "users.manage", new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Create, view, edit, and delete users", "Manage Users" },
                    { 6, "Designs", "designs.manage", new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Upload and manage invitation designs", "Manage Designs" },
                    { 7, "Issuers", "issuers.manage", new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Create and manage issuers", "Manage Issuers" },
                    { 8, "Reports", "reports.view", new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), "View and export reports", "View Reports" },
                    { 9, "Settings", "settings.manage", new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Manage application settings", "Manage Settings" },
                    { 10, "Tickets", "invitations.verify", new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Search and verify invitation codes", "Verify Invitations" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Full system access", "Administrator" },
                    { 2, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Can manage tickets and events", "Manager" },
                    { 3, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Can view and create tickets", "Operator" },
                    { 4, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Can only search and verify invitation codes", "Checker" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "IsActive", "IsAdmin", "LastLoginAt", "LastName", "Password" },
                values: new object[] { 1, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), "admin@gmail.com", "System", true, true, null, "Administrator", "$2a$11$8KQH0vIFZU2ImxhLM7N/vO7pfwFqixeOT7yCw.j1nzSIOjcnHQOli" });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "AssignedAt", "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1 },
                    { 2, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), 2, 1 },
                    { 3, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), 3, 1 },
                    { 4, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), 4, 1 },
                    { 5, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), 5, 1 },
                    { 6, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), 6, 1 },
                    { 7, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), 7, 1 },
                    { 8, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), 8, 1 },
                    { 9, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), 9, 1 },
                    { 10, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), 10, 1 },
                    { 11, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), 1, 2 },
                    { 12, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), 3, 2 },
                    { 13, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), 6, 2 },
                    { 14, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), 7, 2 },
                    { 15, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), 8, 2 },
                    { 16, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), 2, 3 },
                    { 17, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), 4, 3 },
                    { 18, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), 10, 4 }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "AssignedAt", "RoleId", "UserId" },
                values: new object[] { 1, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_EventId",
                table: "Invitations",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_Issued",
                table: "Invitations",
                column: "Issued");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Code",
                table: "Permissions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId",
                table: "RolePermissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationSettings");

            migrationBuilder.DropTable(
                name: "Invitations");

            migrationBuilder.DropTable(
                name: "InvitationsWithPoint");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Issueds");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
