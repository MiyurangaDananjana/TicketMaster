using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketMaster.Migrations
{
    /// <inheritdoc />
    public partial class addnewcl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_InvitationsWithPoint_Issued",
                table: "InvitationsWithPoint",
                column: "Issued");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_Issued",
                table: "Invitations",
                column: "Issued");

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_Issueds_Issued",
                table: "Invitations",
                column: "Issued",
                principalTable: "Issueds",
                principalColumn: "UserCode",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvitationsWithPoint_Issueds_Issued",
                table: "InvitationsWithPoint",
                column: "Issued",
                principalTable: "Issueds",
                principalColumn: "UserCode",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_Issueds_Issued",
                table: "Invitations");

            migrationBuilder.DropForeignKey(
                name: "FK_InvitationsWithPoint_Issueds_Issued",
                table: "InvitationsWithPoint");

            migrationBuilder.DropTable(
                name: "Issueds");

            migrationBuilder.DropIndex(
                name: "IX_InvitationsWithPoint_Issued",
                table: "InvitationsWithPoint");

            migrationBuilder.DropIndex(
                name: "IX_Invitations_Issued",
                table: "Invitations");
        }
    }
}
