using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityEvents.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class createpass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PasswordResetOtp",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Otp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpireAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetOtp", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8b1b2daf-8bbd-4a6e-8ba2-3436e6b842dc", "AQAAAAIAAYagAAAAEOZsvLJfCWQQqcZtIdo2XTQDSbHxrtQ29ZbZcM6WaMP4C/0t6ZinvUvGp/rVUv+kDA==", "6d461a06-210f-42e5-b6b2-e4d55a0f2af2" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b567e2f2-dbae-4b89-bea9-6359951cb062", "AQAAAAIAAYagAAAAEFQcf8AdKSe1L44tddUxKRwGJDMLDitCdC1pKdRjZD7sbi2MXUb6jqTB9qBp6sVKbQ==", "f8ee2ce1-3935-4d91-99ab-6fefe0a5beee" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "80747df5-1c17-46e8-b361-59e3c3a1d01d", "AQAAAAIAAYagAAAAENG+HuVLAd8VFNJDpVwl0YyKQYD3StxyJm4PHGUQyxbbG0HoT2bkeEvSAtvTxAH/BQ==", "b38be033-f950-4e3b-b1c0-9febdbc6fc92" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PasswordResetOtp");

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
        }
    }
}
