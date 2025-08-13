using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketMaster.Migrations
{
    /// <inheritdoc />
    public partial class addnewcoloumncatrgory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserCategory",
                table: "Invitations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserCategory",
                table: "Invitations");
        }
    }
}
