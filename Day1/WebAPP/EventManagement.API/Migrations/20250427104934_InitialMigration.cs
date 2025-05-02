using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EventManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    VenueName = table.Column<string>(type: "TEXT", nullable: false),
                    Capacity = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentEventId = table.Column<int>(type: "INTEGER", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    BannerImageUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_Events_ParentEventId",
                        column: x => x.ParentEventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    EventId = table.Column<int>(type: "INTEGER", nullable: false),
                    SpeakerName = table.Column<string>(type: "TEXT", nullable: false),
                    SpeakerBio = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "Id", "BannerImageUrl", "Capacity", "Description", "EndDate", "ParentEventId", "StartDate", "Status", "Title", "Type", "VenueName" },
                values: new object[,]
                {
                    { 1, "/sample-images/tech-conference.jpg", 1000, "Annual technology conference featuring the latest innovations and industry trends.", new DateTime(2025, 6, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Tech Conference 2025", 1, "Tech Convention Center" },
                    { 4, "/sample-images/summer-concert.jpg", 500, "Annual summer concert featuring local bands and artists.", new DateTime(2025, 7, 15, 22, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2025, 7, 15, 18, 0, 0, 0, DateTimeKind.Unspecified), 0, "Summer Concert", 0, "City Park Amphitheater" },
                    { 2, null, 1000, "Opening keynote discussing the future of artificial intelligence and its impact on society.", new DateTime(2025, 6, 1, 10, 30, 0, 0, DateTimeKind.Unspecified), 1, new DateTime(2025, 6, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), 1, "Keynote: Future of AI", 2, "Main Hall" },
                    { 3, null, 50, "Hands-on workshop on developing applications with Blazor and .NET.", new DateTime(2025, 6, 2, 16, 0, 0, 0, DateTimeKind.Unspecified), 1, new DateTime(2025, 6, 2, 13, 0, 0, 0, DateTimeKind.Unspecified), 1, "Workshop: Building with Blazor", 2, "Workshop Room A" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_ParentEventId",
                table: "Events",
                column: "ParentEventId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_EventId",
                table: "Sessions",
                column: "EventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Events");
        }
    }
}
