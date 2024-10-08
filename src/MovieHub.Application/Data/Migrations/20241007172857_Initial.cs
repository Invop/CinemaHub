using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieHub.Application.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "movies",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    slug = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    yearofrelease = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "genres",
                columns: table => new
                {
                    movieid = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_genres", x => new { x.movieid, x.name });
                    table.ForeignKey(
                        name: "FK_genres_movies_movieid",
                        column: x => x.movieid,
                        principalTable: "movies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ratings",
                columns: table => new
                {
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    movieid = table.Column<Guid>(type: "uuid", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ratings", x => new { x.userid, x.movieid });
                    table.ForeignKey(
                        name: "FK_ratings_movies_movieid",
                        column: x => x.movieid,
                        principalTable: "movies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "movies_slug_idx",
                table: "movies",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ratings_movieid",
                table: "ratings",
                column: "movieid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "genres");

            migrationBuilder.DropTable(
                name: "ratings");

            migrationBuilder.DropTable(
                name: "movies");
        }
    }
}
