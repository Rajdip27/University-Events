using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityEvents.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRegister : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "StudentRegistrations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9833e4b9-6ec3-468c-b13a-5b93454fa740", "AQAAAAIAAYagAAAAEOTFxi4W+Zg0IycHk0coNj3HEkAdCUexqzLk9Ob6PQ7+sHplqiqrWjB7oehKVHt1Pw==", "2060b7c8-d916-48a2-b2d2-823921e8739a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cc9b87ff-7ebe-4b2a-8705-040949094f26", "AQAAAAIAAYagAAAAEIMuXzcvss9HC9Vp+JlBU+C4KRp0Fl3mHF1PIt+WrHSxrrS1yJJA3b9vfb293PyMfA==", "f902ded5-69f8-4fa0-898c-d317bb6a44dd" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bdf14920-bb6a-423c-8d3c-9b03ba35d094", "AQAAAAIAAYagAAAAEIMJj/E9mBnWiJRN7aC5znlC9m3+8oZA2Kta5acbmaxNLvbrHS+ApnLlIBHWS+D2Pg==", "6785e748-f990-4f6e-9f46-2979c28ec249" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3bb9b310-ffae-4b6c-a8ff-f730d719e26b", "AQAAAAIAAYagAAAAEHUXbWgvYn+cKzH30cTiOdb81cbmV1VUpp6Hnh7Y+FWK3KbYhl5NYOZk+D8JP9XZxA==", "e0f8a586-03e6-4e23-b74a-184e72c7503d" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "StudentRegistrations");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "64e1ac80-ced5-4528-9dc0-15ce044db465", "AQAAAAIAAYagAAAAEFr+GioVOzAkUzpdWoygwlhvV9kPodyi+Of3x7c9WjpcM4vAJkGV68xXlHXj+2HqzA==", "4e60a27f-82c9-4988-9795-e94416e28b02" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "faabac8e-d159-4fba-a3ed-f31c49641c8c", "AQAAAAIAAYagAAAAEN81Ed0xVCNPgp7QloOVJPIjKJ1mgnO88f7CwlbIMvoqzBbKgwU55ei1l5j+bStCJA==", "243a149b-ae37-4a10-91c3-713541f3dcc0" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f6204e1c-4827-4626-8607-a554b3c3c805", "AQAAAAIAAYagAAAAELzXPW4uBwlmodQZOzz9+9bEIb7zbNLgDl9GZ3s3VYlBDb1NgiWr5I7IA0fAlQkV1g==", "911315f4-2126-4341-8ff9-efee08291ee1" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9a0cfcae-e5d7-42cb-8df9-69090d42caf6", "AQAAAAIAAYagAAAAEPx+5xeY7/1K5AiroBJwyE4iCZR1uo7idC8qzlmuvHIlXluAzeiu+ZSQxjQHkaawtg==", "a77dfa54-3a95-4f4b-a1a1-c84842eeaa61" });
        }
    }
}
