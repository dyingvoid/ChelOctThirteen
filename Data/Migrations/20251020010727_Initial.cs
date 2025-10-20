using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "delta_archive",
                columns: table => new
                {
                    version_id = table.Column<int>(type: "integer", nullable: false),
                    archive_link = table.Column<string>(type: "text", nullable: false),
                    exp_date = table.Column<DateOnly>(type: "date", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    downloaded_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()"),
                    UnzippedFilesPath = table.Column<string>(type: "text", nullable: true),
                    unarchived_at = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_delta_archive", x => x.version_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "delta_archive");
        }
    }
}
