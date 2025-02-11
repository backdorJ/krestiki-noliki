using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RPS.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AlterGameTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "WhoCreatedName",
                table: "Games",
                type: "text",
                nullable: false,
                defaultValue: "RPS",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "RoomName",
                table: "Games",
                type: "text",
                nullable: false,
                defaultValue: "RPS Room",
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "TicTacToe Room");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Rating",
                table: "Users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "WhoCreatedName",
                table: "Games",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "RPS");

            migrationBuilder.AlterColumn<string>(
                name: "RoomName",
                table: "Games",
                type: "text",
                nullable: false,
                defaultValue: "TicTacToe Room",
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "RPS Room");
        }
    }
}
