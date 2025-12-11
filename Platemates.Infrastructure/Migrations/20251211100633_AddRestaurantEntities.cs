using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Platemates.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRestaurantEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Restaurants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GooglePlaceId = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Address = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    CuisineType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Website = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Longitude = table.Column<double>(type: "double precision", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Restaurants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRestaurants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRestaurants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRestaurants_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRestaurants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestaurantReviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserRestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    PriceRange = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestaurantReviews_UserRestaurants_UserRestaurantId",
                        column: x => x.UserRestaurantId,
                        principalTable: "UserRestaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantReviews_UserRestaurantId",
                table: "RestaurantReviews",
                column: "UserRestaurantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Restaurants_GooglePlaceId",
                table: "Restaurants",
                column: "GooglePlaceId",
                unique: true,
                filter: "\"GooglePlaceId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserRestaurants_RestaurantId",
                table: "UserRestaurants",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRestaurants_UserId_RestaurantId",
                table: "UserRestaurants",
                columns: new[] { "UserId", "RestaurantId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RestaurantReviews");

            migrationBuilder.DropTable(
                name: "UserRestaurants");

            migrationBuilder.DropTable(
                name: "Restaurants");
        }
    }
}
