using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityEvents.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderId = table.Column<long>(type: "bigint", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessageReceivers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChatMessageId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "0"),
                    ReceiverId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessageReceivers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessageReceivers_ChatMessages_ChatMessageId",
                        column: x => x.ChatMessageId,
                        principalTable: "ChatMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "96fe1927-4c42-4b09-af2a-1b1f330a0198", "AQAAAAIAAYagAAAAEGJUo9gxLoOJFLw37AR1yL4c1WKNmqE8Cg7ReMIxbOETb8rPyqRrbxMY3Yd8d5ByhA==", "fb7d36f9-19bb-4c38-bfed-c5665374bfb9" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "403c7d4d-5d22-474a-968c-f3c7badb7692", "AQAAAAIAAYagAAAAENVAefnNrhRz+NQADC2k3yzAOTzqsTlFb+CzXM8HjRKt+ZiS26PcZvTFszZioJqCWg==", "3d2279b5-d7e8-436e-83e9-c6a86c0c08ec" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a945d804-c388-4d14-b582-df54d1d8854e", "AQAAAAIAAYagAAAAEIsuUajtH4s0bkehsu6gM4dx9+QJWeUaCnli2YnS52qyX0oL9u9xkhjosbnuBZLcHQ==", "4aaaa298-3d3f-4c37-b139-5b23e13cd6d6" });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessageReceivers_ChatMessageId",
                table: "ChatMessageReceivers",
                column: "ChatMessageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMessageReceivers");

            migrationBuilder.DropTable(
                name: "ChatMessages");

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
    }
}
