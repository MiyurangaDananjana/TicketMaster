using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TicketMaster.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "application_settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ColorCode = table.Column<string>(type: "text", nullable: false),
                    FrontSize = table.Column<string>(type: "text", nullable: false),
                    FontFamily = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application_settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EventDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    MaxTickets = table.Column<int>(type: "integer", nullable: false),
                    ImagePath = table.Column<string>(type: "text", nullable: true),
                    CreatedTicketsCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "invitations_with_point",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Issued = table.Column<string>(type: "text", nullable: false),
                    ImagePath = table.Column<string>(type: "text", nullable: false),
                    CoordinateX = table.Column<int>(type: "integer", nullable: false),
                    CoordinateY = table.Column<int>(type: "integer", nullable: false),
                    InvitationType = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invitations_with_point", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "role_permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    PermissionId = table.Column<int>(type: "integer", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_role_permissions_permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_role_permissions_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "issueds",
                columns: table => new
                {
                    UserCode = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_issueds", x => x.UserCode);
                    table.ForeignKey(
                        name: "FK_issueds_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_roles_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_roles_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "invitations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InviterName = table.Column<string>(type: "text", nullable: false),
                    InvitationType = table.Column<string>(type: "text", nullable: false),
                    Issued = table.Column<string>(type: "text", nullable: false),
                    UniqCode = table.Column<string>(type: "text", nullable: false),
                    ImagePath = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EventId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_invitations_events_EventId",
                        column: x => x.EventId,
                        principalTable: "events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_invitations_issueds_Issued",
                        column: x => x.Issued,
                        principalTable: "issueds",
                        principalColumn: "UserCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "permissions",
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
                table: "roles",
                columns: new[] { "Id", "CreatedAt", "Description", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Full system access", "Administrator" },
                    { 2, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Can manage tickets and events", "Manager" },
                    { 3, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Can view and create tickets", "Operator" },
                    { 4, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Can only search and verify invitation codes", "Checker" }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "IsActive", "IsAdmin", "LastLoginAt", "LastName", "Password" },
                values: new object[] { 1, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), "admin@gmail.com", "System", true, true, null, "Administrator", "$2a$11$k9Vm4Rbs46A..TH6lXiHsuMcEu/UaEDwZqpWRhtmClre5012CuYXu" });

            migrationBuilder.InsertData(
                table: "role_permissions",
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
                table: "user_roles",
                columns: new[] { "Id", "AssignedAt", "RoleId", "UserId" },
                values: new object[] { 1, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_invitations_EventId",
                table: "invitations",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_invitations_Issued",
                table: "invitations",
                column: "Issued");

            migrationBuilder.CreateIndex(
                name: "IX_issueds_UserId",
                table: "issueds",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_permissions_Code",
                table: "permissions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_PermissionId",
                table: "role_permissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_RoleId",
                table: "role_permissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_roles_Name",
                table: "roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_RoleId",
                table: "user_roles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_UserId",
                table: "user_roles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_users_Email",
                table: "users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "application_settings");

            migrationBuilder.DropTable(
                name: "invitations");

            migrationBuilder.DropTable(
                name: "invitations_with_point");

            migrationBuilder.DropTable(
                name: "role_permissions");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "events");

            migrationBuilder.DropTable(
                name: "issueds");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
