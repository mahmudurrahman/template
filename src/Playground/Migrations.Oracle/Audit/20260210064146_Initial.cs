using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSH.Playground.Migrations.Oracle.Audit
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OccurredAtUtc = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ReceivedAtUtc = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    EventType = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Severity = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    TenantId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    UserId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    UserName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TraceId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SpanId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CorrelationId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    RequestId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Source = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Tags = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    PayloadJson = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditRecords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditRecords_EventType",
                table: "AuditRecords",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_AuditRecords_OccurredAtUtc",
                table: "AuditRecords",
                column: "OccurredAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_AuditRecords_TenantId",
                table: "AuditRecords",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditRecords");
        }
    }
}
