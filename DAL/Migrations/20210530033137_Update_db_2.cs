using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class Update_db_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UrlName",
                table: "AppProductCategories",
                maxLength: 60,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2301D884-221A-4E7D-B509-0113DCC043E2",
                column: "ConcurrencyStamp",
                value: "033e681c-650e-407b-8d7a-fc67b8cf03f6");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "B22698B8-42A2-4115-9631-1C2D1E2AC5F8",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "63a6e189-a00b-4ab2-a409-2762b0051ec0", "AQAAAAEAACcQAAAAEHtv6rvlvfda2zrsAepxFDrQIpmc2/h/bi8mqUQKrG0POpgTltfDq5if7CGBS3zR9Q==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrlName",
                table: "AppProductCategories");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2301D884-221A-4E7D-B509-0113DCC043E2",
                column: "ConcurrencyStamp",
                value: "93c171be-ce54-4465-8b97-86bf76ba0446");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "B22698B8-42A2-4115-9631-1C2D1E2AC5F8",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "49aaaa55-5197-4e4a-9f5b-c95352000591", "AQAAAAEAACcQAAAAEGqCwBfvN7f5Qyq6GWUmTRFF7lKUxXUeUnp+FGZfxYPYbyIAWXWFjicIstLJV6xZRw==" });
        }
    }
}
