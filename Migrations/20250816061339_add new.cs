using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketMaster.Migrations
{
    /// <inheritdoc />
    public partial class addnew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "IssuedUserCode",
                table: "InvitationsWithPoint",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvitationsWithPoint_IssuedUserCode",
                table: "InvitationsWithPoint",
                column: "IssuedUserCode");

            migrationBuilder.AddForeignKey(
                name: "FK_InvitationsWithPoint_Issueds_IssuedUserCode",
                table: "InvitationsWithPoint",
                column: "IssuedUserCode",
                principalTable: "Issueds",
                principalColumn: "UserCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvitationsWithPoint_Issueds_IssuedUserCode",
                table: "InvitationsWithPoint");

            migrationBuilder.DropIndex(
                name: "IX_InvitationsWithPoint_IssuedUserCode",
                table: "InvitationsWithPoint");

            migrationBuilder.DropColumn(
                name: "IssuedUserCode",
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
    }
}
