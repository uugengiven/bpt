using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BrokenPictureTelephone.Data.Migrations
{
    public partial class FixDateLastServed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateLastServed",
                table: "Entries");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateLastServed",
                table: "Games",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateLastServed",
                table: "Games");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateLastServed",
                table: "Entries",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
