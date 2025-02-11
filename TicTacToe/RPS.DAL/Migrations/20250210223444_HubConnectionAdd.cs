using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RPS.DAL.Migrations
{
    /// <inheritdoc />
    public partial class HubConnectionAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HubConnection",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HubConnection",
                table: "Users");
        }
    }
}
