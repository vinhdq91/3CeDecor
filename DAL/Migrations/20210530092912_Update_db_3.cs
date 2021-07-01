using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class Update_db_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProvinceCode",
                table: "Ward",
                maxLength: 60,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2301D884-221A-4E7D-B509-0113DCC043E2",
                column: "ConcurrencyStamp",
                value: "75dd712c-49f2-4003-87d0-42a083863098");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "B22698B8-42A2-4115-9631-1C2D1E2AC5F8",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "715256a7-4e2b-4761-bf21-2e8033279bfd", "AQAAAAEAACcQAAAAEETavye4hpAiiZByErNSL7ld59mNSKCfv7ILPToNFQ9y3aBra5bAAbB1kmP1AXnVQw==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProvinceCode",
                table: "Ward");

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
    }
}
