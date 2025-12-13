using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketMaster.Migrations
{
    /// <inheritdoc />
    public partial class addnewclnew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvitationsWithPoint_Issueds_Issued",
                table: "InvitationsWithPoint");

            migrationBuilder.DropIndex(
                name: "IX_InvitationsWithPoint_Issued",
                table: "InvitationsWithPoint");

            migrationBuilder.AddColumn<string>(
                name: "IssuedUserUserCode",
                table: "InvitationsWithPoint",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationsWithPoint_IssuedUserUserCode",
                table: "InvitationsWithPoint",
                column: "IssuedUserUserCode");

            migrationBuilder.AddForeignKey(
                name: "FK_InvitationsWithPoint_Issueds_IssuedUserUserCode",
                table: "InvitationsWithPoint",
                column: "IssuedUserUserCode",
                principalTable: "Issueds",
                principalColumn: "UserCode",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvitationsWithPoint_Issueds_IssuedUserUserCode",
                table: "InvitationsWithPoint");

            migrationBuilder.DropIndex(
                name: "IX_InvitationsWithPoint_IssuedUserUserCode",
                table: "InvitationsWithPoint");

            migrationBuilder.DropColumn(
                name: "IssuedUserUserCode",
                table: "InvitationsWithPoint");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationsWithPoint_Issued",
                table: "InvitationsWithPoint",
                column: "Issued");

            migrationBuilder.AddForeignKey(
                name: "FK_InvitationsWithPoint_Issueds_Issued",
                table: "InvitationsWithPoint",
                column: "Issued",
                principalTable: "Issueds",
                principalColumn: "UserCode",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
