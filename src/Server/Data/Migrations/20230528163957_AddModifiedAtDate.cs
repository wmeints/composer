using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Composer.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddModifiedAtDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Proposals",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Proposals");
        }
    }
}
