using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IMDbClone.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "Id", "Cast", "CreatedAt", "Director", "Duration", "Genre", "Language", "PosterUrl", "ReleaseDate", "Synopsis", "Title", "TrailerUrl" },
                values: new object[,]
                {
                    { 1, "Leonardo DiCaprio, Joseph Gordon-Levitt, Ellen Page", new DateTime(2024, 10, 16, 19, 57, 37, 577, DateTimeKind.Utc).AddTicks(9969), "Christopher Nolan", 148, 1, "English", "http://example.com/inception.jpg", new DateTime(2010, 7, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "A thief who steals corporate secrets through the use of dream-sharing technology is given the inverse task of planting an idea into the mind of a CEO.", "Inception", "http://example.com/inception_trailer.mp4" },
                    { 2, "Keanu Reeves, Laurence Fishburne, Carrie-Anne Moss", new DateTime(2024, 10, 16, 19, 57, 37, 577, DateTimeKind.Utc).AddTicks(9979), "The Wachowskis", 136, 18, "English", "http://example.com/the_matrix.jpg", new DateTime(1999, 3, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "A computer hacker learns from mysterious rebels about the true nature of his reality and his role in the war against its controllers.", "The Matrix", "http://example.com/the_matrix_trailer.mp4" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Password", "UserName" },
                values: new object[,]
                {
                    { 1, "password1", "user1" },
                    { 2, "password2", "user2" }
                });

            migrationBuilder.InsertData(
                table: "Ratings",
                columns: new[] { "Id", "CreatedAt", "MovieId", "Score", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 10, 16, 19, 57, 37, 578, DateTimeKind.Utc).AddTicks(11), 1, 9m, 1 },
                    { 2, new DateTime(2024, 10, 16, 19, 57, 37, 578, DateTimeKind.Utc).AddTicks(17), 1, 8m, 2 },
                    { 3, new DateTime(2024, 10, 16, 19, 57, 37, 578, DateTimeKind.Utc).AddTicks(20), 2, 10m, 1 }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "Content", "CreatedAt", "MovieId", "UserId" },
                values: new object[,]
                {
                    { 1, "Amazing movie!", new DateTime(2024, 10, 16, 19, 57, 37, 578, DateTimeKind.Utc).AddTicks(48), 1, 1 },
                    { 2, "Really good!", new DateTime(2024, 10, 16, 19, 57, 37, 578, DateTimeKind.Utc).AddTicks(53), 1, 2 },
                    { 3, "A groundbreaking film.", new DateTime(2024, 10, 16, 19, 57, 37, 578, DateTimeKind.Utc).AddTicks(54), 2, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Ratings",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Ratings",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Ratings",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
