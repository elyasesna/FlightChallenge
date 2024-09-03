using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightChallenge.Migrations
{
    /// <inheritdoc />
    public partial class addindex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Routes_DepartureDate_OriginCityId_DestinationCityId",
                table: "Routes",
                columns: new[] { "DepartureDate", "OriginCityId", "DestinationCityId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Routes_DepartureDate_OriginCityId_DestinationCityId",
                table: "Routes");
        }
    }
}
