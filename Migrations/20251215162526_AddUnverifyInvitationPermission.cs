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
            // Add new permission for unverifying invitations
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Category", "Code", "CreatedAt", "Description", "Name" },
                values: new object[] { 11, "Invitations", "invitations.unverify", new DateTime(2025, 12, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Remove verification status from invitations", "Unverify Invitations" });

            // Assign the new permission to Administrator role (RoleId = 1)
            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "AssignedAt", "PermissionId", "RoleId" },
                values: new object[] { 19, new DateTime(2025, 12, 15, 0, 0, 0, 0, DateTimeKind.Utc), 11, 1 });

            // Update admin password hash (from EF Core tracking changes)
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$hM7Bw8H6jKs0EKr0Y0I1gemNl.x2MYWkm0tAWH5.nSz7f9ienaYly");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the role permission assignment
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 19);

            // Remove the new permission
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$H.ABdoe1nv2NEtxblDdhT.nZHk0WeC.tVM7vNRKFzHp2YTSMOWQhy");
        }
    }
}
