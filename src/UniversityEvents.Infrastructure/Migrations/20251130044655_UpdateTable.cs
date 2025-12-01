using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityEvents.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "StudentRegistrations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "82cac9c4-9f1a-4bb9-9b62-f7d692c51c8c", "AQAAAAIAAYagAAAAEMXB9NDzXDLcvuR8aaONQLHy0fX9Pvr7A7u0EMtZBRMWOGqmc++qKMk9tvKJ6KbsEw==", "de3d6b00-9624-4334-86c4-2f484b2eeca1" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cbd57101-a66c-4404-b0b8-62635ed43e4f", "AQAAAAIAAYagAAAAEATiyEwcT2es3E1HcsO5zP8WHLYQfBeow/iSZswnbsEZQZzHDqIwycGB8TpmnuK2iQ==", "82d3e3b3-0c48-4eb9-99b9-e89c8d850946" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a3a62a76-7116-4b2d-8225-8f3879b0b343", "AQAAAAIAAYagAAAAEKPInDKmHqgAI0AoFu3K9/uc+0cviAbXyZtxqozl7/yMI6HUAEPvbw3lfKO5czIUQg==", "54e68aff-f969-40e6-b608-3d01639ce601" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "750b9efb-0a12-473b-a210-4e9f17fbcda7", "AQAAAAIAAYagAAAAEArYhPCf/YxcJll8PEvFAunz81PoLr0o+wH4W1W/W3kmeV7NSyx9lVsYgResyzuW8w==", "d10f85c7-58ff-4954-a0a0-aa441c43bab3" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "StudentRegistrations");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "Payments");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8889b395-92f7-4946-8a8f-c8986bc87c2f", "AQAAAAIAAYagAAAAENPABExTlxvzHNlSORr2xEBwLUOcsqxNVU0c6SYbvmFJc/hCNxg/fObU0XhFYBjRqw==", "95138dd1-6612-499b-9f5e-5c0ce270bc3d" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6a6fb9f0-2a98-4c11-8e52-7d51877c0f71", "AQAAAAIAAYagAAAAEH9wjK7pfz9lTBYsg6+uhdJk/A7ZfVpcLkbA5isAan1M1wX22yPMr48bZemxIcBO+g==", "c6473c63-3a12-441b-998f-886aab2c9ba5" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "568d9792-8dd9-4c19-99f9-6aa7a0d86db1", "AQAAAAIAAYagAAAAECoV9v7QPAzrz4xnGMoQzYVfkfpdU5aRS45pdREl6li/cNAg2uPU6WP5Kd+okEUiSQ==", "1290de17-80d0-4064-b40e-75b60a9b4523" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5cd18fff-8fc1-4acc-83bc-f0fe7af161d3", "AQAAAAIAAYagAAAAEKOE+bttuDmbDSmitlFuFjunGKQJ5EtV4StYFiOMAPv4HwFIxDURsWHSJ76Wx/52Hg==", "3b0ea99a-0d76-4f91-83b2-67beb0a79fb5" });
        }
    }
}
