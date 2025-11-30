using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityEvents.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "StudentRegistrations",
                type: "nvarchar(max)",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "StudentRegistrations");

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
    }
}
