using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePulse.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProviderAndFallbackToAnalysisResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FallbackReason",
                table: "AnalysisResults",
                type: "nvarchar(1500)",
                maxLength: 1500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Provider",
                table: "AnalysisResults",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "RuleBased");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FallbackReason",
                table: "AnalysisResults");

            migrationBuilder.DropColumn(
                name: "Provider",
                table: "AnalysisResults");
        }
    }
}
