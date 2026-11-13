using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePulse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ExternalReference = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    ContactMasked = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Agents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    EmployeeCode = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    TeamName = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Role = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    RowVersion = table.Column<byte[]>(type: "BLOB", nullable: true),
                    AgentId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationUsers_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Calls",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AgentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ExternalCallId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    StartedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DurationSeconds = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    Status = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    ResolutionStatus = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Calls_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Calls_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnalysisResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AnalyzerVersion = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Sentiment = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    RiskLevel = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    QualityScore = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Summary = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    DetectedKeywordsJson = table.Column<string>(type: "TEXT", nullable: true),
                    AnalysisStatus = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", maxLength: 1500, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CallId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnalysisResults_Calls_CallId",
                        column: x => x.CallId,
                        principalTable: "Calls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    EntityName = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    EntityId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Action = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    ActorUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CorrelationId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DetailsJson = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CallId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_ApplicationUsers_ActorUserId",
                        column: x => x.ActorUserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AuditLogs_Calls_CallId",
                        column: x => x.CallId,
                        principalTable: "Calls",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CallTranscripts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CallId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    Source = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    LanguageCode = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false, defaultValue: "en"),
                    WordCount = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CallTranscripts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CallTranscripts_Calls_CallId",
                        column: x => x.CallId,
                        principalTable: "Calls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ManagerReviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ReviewStatus = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    ManagerScore = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    CoachingNotes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    OverrideReason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ReviewedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReviewerUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CallId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagerReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManagerReviews_ApplicationUsers_ReviewerUserId",
                        column: x => x.ReviewerUserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ManagerReviews_Calls_CallId",
                        column: x => x.CallId,
                        principalTable: "Calls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agents_EmployeeCode",
                table: "Agents",
                column: "EmployeeCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Agents_UserId",
                table: "Agents",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisResults_CallId",
                table: "AnalysisResults",
                column: "CallId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUsers_AgentId",
                table: "ApplicationUsers",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ActorUserId",
                table: "AuditLogs",
                column: "ActorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CallId",
                table: "AuditLogs",
                column: "CallId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityName_EntityId",
                table: "AuditLogs",
                columns: new[] { "EntityName", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_Calls_AgentId",
                table: "Calls",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Calls_CustomerId",
                table: "Calls",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Calls_StartedAtUtc",
                table: "Calls",
                column: "StartedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Calls_Status",
                table: "Calls",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CallTranscripts_CallId",
                table: "CallTranscripts",
                column: "CallId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_ExternalReference",
                table: "Customers",
                column: "ExternalReference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ManagerReviews_CallId",
                table: "ManagerReviews",
                column: "CallId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagerReviews_ReviewerUserId",
                table: "ManagerReviews",
                column: "ReviewerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agents_ApplicationUsers_UserId",
                table: "Agents",
                column: "UserId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agents_ApplicationUsers_UserId",
                table: "Agents");

            migrationBuilder.DropTable(
                name: "AnalysisResults");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "CallTranscripts");

            migrationBuilder.DropTable(
                name: "ManagerReviews");

            migrationBuilder.DropTable(
                name: "Calls");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "ApplicationUsers");

            migrationBuilder.DropTable(
                name: "Agents");
        }
    }
}
