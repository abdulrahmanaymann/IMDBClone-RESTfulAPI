using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMDbClone.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class someIndexing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Movies_Director_Genre",
                table: "Movies",
                columns: new[] { "Director", "Genre" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Movies_Director_Genre",
                table: "Movies");
        }
    }
}
