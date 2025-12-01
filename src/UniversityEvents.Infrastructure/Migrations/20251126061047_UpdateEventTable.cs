using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityEvents.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEventTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Events");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1b89e285-7520-43fb-9efe-da4b9d9a8c3a", "AQAAAAIAAYagAAAAEEv6mW9CR9Rw59/JLVvEdfAH7+EQ0jt7+KJTCJ2k/4HBlXyHzQaLrtIil1oe7tJeCg==", "0d0c2d9c-319d-4476-8485-c4733d0d00cd" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2b3683fb-d5e7-4d03-83b0-1a55f4cedcd9", "AQAAAAIAAYagAAAAEIwWzciyl4HRUIjJwLuF3KfREiI23jGNLtYQ5B2rHY80U5InpZEjxBzUfhYSCrIEVA==", "6d684786-d0ff-4bff-835f-03ff9279422a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "83545861-b112-467e-a321-839bf9b49102", "AQAAAAIAAYagAAAAECmg9Euf27spW/KKa1cEf5OBxWZnKbYBg4CDY/OvzzHZbHgPT3EdxQD3asJl/krGCA==", "6746040e-3339-4a88-a826-8abf2caeea3c" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "34feb217-c35d-4a08-b209-a9e48c521274", "AQAAAAIAAYagAAAAELdtkasl7Fo+kkOg+t/w5TxWAAArUEoAglQed5ipLEj+4Gg+Xfun0OnDfGF4kSMd4A==", "6758b5d2-00ab-480d-9a4a-da53e2ca9f3a" });
        }
    }
}
