using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERA.Host.Migrations.Oracle.MultiTenancy
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TenantProvisionings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TenantId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CorrelationId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Status = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CurrentStep = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Error = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    JobId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    StartedUtc = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    CompletedUtc = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantProvisionings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ConnectionString = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    AdminEmail = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    IsActive = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ValidUpto = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Issuer = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Identifier = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantThemes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TenantId = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: true),
                    PrimaryColor = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
                    SecondaryColor = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
                    TertiaryColor = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
                    BackgroundColor = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
                    SurfaceColor = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
                    ErrorColor = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
                    WarningColor = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
                    SuccessColor = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
                    InfoColor = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
                    DarkPrimaryColor = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
                    DarkSecondaryColor = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
                    DarkTertiaryColor = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
                    DarkBackgroundColor = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
                    DarkSurfaceColor = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
                    DarkErrorColor = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
                    DarkWarningColor = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
                    DarkSuccessColor = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
                    DarkInfoColor = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
                    LogoUrl = table.Column<string>(type: "NCLOB", maxLength: 2048, nullable: true),
                    LogoDarkUrl = table.Column<string>(type: "NCLOB", maxLength: 2048, nullable: true),
                    FaviconUrl = table.Column<string>(type: "NCLOB", maxLength: 2048, nullable: true),
                    FontFamily = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    HeadingFontFamily = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    FontSizeBase = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    LineHeightBase = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    BorderRadius = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: true),
                    DefaultElevation = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    IsDefault = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CreatedOnUtc = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    CreatedBy = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    LastModifiedOnUtc = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantThemes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantProvisioningSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProvisioningId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Step = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Status = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Error = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    StartedUtc = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    CompletedUtc = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantProvisioningSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantProvisioningSteps_TenantProvisionings_ProvisioningId",
                        column: x => x.ProvisioningId,
                        principalTable: "TenantProvisionings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenantProvisioningSteps_ProvisioningId",
                table: "TenantProvisioningSteps",
                column: "ProvisioningId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Identifier",
                table: "Tenants",
                column: "Identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantThemes_TenantId",
                table: "TenantThemes",
                column: "TenantId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantProvisioningSteps");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropTable(
                name: "TenantThemes");

            migrationBuilder.DropTable(
                name: "TenantProvisionings");
        }
    }
}
