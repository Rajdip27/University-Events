using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityEvents.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Createpaymenthistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ValidationId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PaymentHistory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "0"),
                    Provider = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProviderSessionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValidationId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerMobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JsonResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentHistory_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "16e7940e-028d-4d01-939f-b93045134188", "AQAAAAIAAYagAAAAEEWmNB8SNpusOrD1WB5ftJQHtqFyzIF6xxsTm6r9YevYGQCoGFB17DR1s+9xyWsWAw==", "1ac35be1-9f0d-4069-b6eb-f381d3a66064" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2953a2a0-2ba2-4bac-accb-5537a0c3ff15", "AQAAAAIAAYagAAAAECUEdFFJrTFTUgsoHGmtQXP85H2luQNxr6NRKXeg1Naa63gAABSuhSAw6kX1z/8bew==", "cfa5c3ed-c0e4-4286-b72e-8d36d5822b28" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "48508a76-9004-47a1-be27-fc98186ea531", "AQAAAAIAAYagAAAAECvDUYvIa8vPtMxs3Bxx8nuzIYPl4QoNRNL3qRHoxBSUXecKUJKXSHzXn77VTEOWeQ==", "b8364815-1c9e-4502-8a53-e7266263ba83" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentHistory_PaymentId",
                table: "PaymentHistory",
                column: "PaymentId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentHistory");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ValidationId",
                table: "Payments");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7766bf49-0d50-465a-b8d8-6afceaa031dc", "AQAAAAIAAYagAAAAEIR5f47lb+cphhBM5c5EcIU+WcyFe0zGMQ+NvtPMVrg/kFUZk4nNIiLZqfwfG1YNQQ==", "8c8ec0ee-3335-4d3a-8351-67fa454dd0e8" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f9c6c4f6-d437-47ad-9915-ab0d95e6af7f", "AQAAAAIAAYagAAAAEEF7ymG1Ky4T52d7NBEY0WqN/YrDPbs7IEaGJxaDfhA+Ca5kGIEMoGn6gNki4tc7+A==", "8c088cbb-24e4-49e6-94a9-b8f4bd05aa21" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "157caa41-70c0-4cec-ac3a-cc3125143491", "AQAAAAIAAYagAAAAEPfuzUI8BHuCT7zKkyCgx8Zw6yFQrRhCnhsN6y1LaX2Gs6sV1KRBkzz65G63co7lGg==", "47a9736d-35d8-4fef-9cba-cb37ead71e57" });
        }
    }
}
