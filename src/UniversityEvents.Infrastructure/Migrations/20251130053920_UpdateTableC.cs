using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityEvents.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFree",
                table: "Events",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MealsOffered",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFree",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "MealsOffered",
                table: "Events");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c9042791-2211-474d-ba0e-1e301093cbf2", "AQAAAAIAAYagAAAAEBw835QxmCqE/4Q1WvEfhblqdG6LjINfpvjX/HMrfx2rOv33WsgUblneWt7E0vH7XA==", "31366f02-9f5f-4f20-b764-12357c2b75e3" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "357e2d3c-6a9d-494f-a6a4-361ac7998e12", "AQAAAAIAAYagAAAAEIeOaPE1ulFfMNNmJBloWwFCQSqzgUS7WuoiWUKWhQspnZcOB+ou+HP+We18vivunQ==", "c72c2a8d-d06f-4269-b283-145b29216e69" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b2d7bfb3-cfe5-4b7a-a4ac-76231e3c0750", "AQAAAAIAAYagAAAAEG1EUMguQCG57totE1FnQiFYQaP6tGIGt9/feXLj+LvIN9XLupl0FdbkZjhyGDjoKg==", "7a06e1e0-b7c7-4769-8b31-f5706eaefe20" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "50a5eca7-ad69-4a8f-9850-294bdb5a4dba", "AQAAAAIAAYagAAAAEJGxGVD4yVsuccaS8jTVL+Bw3busQILKJavqR8TWWq3X6OfyZ1MHEaIB3ORstw4hwA==", "69da27f7-e75f-4086-8e2f-3ae9e738a161" });
        }
    }
}
