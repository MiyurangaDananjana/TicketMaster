using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketMaster.Migrations
{
    /// <inheritdoc />
    public partial class AddUnverifyInvitationPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastLoginAt",
                table: "users",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "users",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AssignedAt",
                table: "user_roles",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "roles",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AssignedAt",
                table: "role_permissions",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "permissions",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "issueds",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "VerifiedAt",
                table: "invitations",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedAt",
                table: "invitations",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "invitations",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EventDate",
                table: "events",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "events",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "Id", "Category", "Code", "CreatedAt", "Description", "Name" },
                values: new object[] { 11, "Tickets", "invitations.unverify", new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Unverify previously verified invitations", "Unverify Invitations" });

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 11, 1 });

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumn: "Id",
                keyValue: 12,
                column: "PermissionId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumn: "Id",
                keyValue: 13,
                column: "PermissionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumn: "Id",
                keyValue: 14,
                column: "PermissionId",
                value: 6);

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumn: "Id",
                keyValue: 15,
                column: "PermissionId",
                value: 7);

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 8, 2 });

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumn: "Id",
                keyValue: 17,
                column: "PermissionId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 4, 3 });

            migrationBuilder.InsertData(
                table: "role_permissions",
                columns: new[] { "Id", "AssignedAt", "PermissionId", "RoleId" },
                values: new object[] { 19, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), 10, 4 });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$Wp73IY.V2SMmIhbI1nUIiuHL7YvFjXW67cfZ8d5qUG6S9h9vgNhiG");

            migrationBuilder.InsertData(
                table: "role_permissions",
                columns: new[] { "Id", "AssignedAt", "PermissionId", "RoleId" },
                values: new object[] { 20, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Utc), 11, 4 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastLoginAt",
                table: "users",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "users",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AssignedAt",
                table: "user_roles",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "roles",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AssignedAt",
                table: "role_permissions",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "permissions",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "issueds",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "VerifiedAt",
                table: "invitations",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedAt",
                table: "invitations",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "invitations",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EventDate",
                table: "events",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "events",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 1, 2 });

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumn: "Id",
                keyValue: 12,
                column: "PermissionId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumn: "Id",
                keyValue: 13,
                column: "PermissionId",
                value: 6);

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumn: "Id",
                keyValue: 14,
                column: "PermissionId",
                value: 7);

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumn: "Id",
                keyValue: 15,
                column: "PermissionId",
                value: 8);

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 2, 3 });

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumn: "Id",
                keyValue: 17,
                column: "PermissionId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "role_permissions",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { 10, 4 });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$k9Vm4Rbs46A..TH6lXiHsuMcEu/UaEDwZqpWRhtmClre5012CuYXu");
        }
    }
}
