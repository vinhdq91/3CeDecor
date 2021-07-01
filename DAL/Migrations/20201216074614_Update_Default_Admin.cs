using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class Update_Default_Admin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2301D884-221A-4E7D-B509-0113DCC043E2",
                column: "ConcurrencyStamp",
                value: "af082d6a-9410-45f5-8b40-33a28a4851b9");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "B22698B8-42A2-4115-9631-1C2D1E2AC5F8",
                columns: new[] { "ConcurrencyStamp", "IsAdminUser", "PasswordHash" },
                values: new object[] { "45ff1768-7cf1-4fd7-8540-c604b66fbac8", true, "AQAAAAEAACcQAAAAECtRdK08pVfUzxBeSZfOrRI7xOJYg8sHlN6dl8Z16/4PVsYwO1k++SyMTYRtAY+9Xw==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2301D884-221A-4E7D-B509-0113DCC043E2",
                column: "ConcurrencyStamp",
                value: "a3b20f04-a72f-4583-98ae-74c69d69252c");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "B22698B8-42A2-4115-9631-1C2D1E2AC5F8",
                columns: new[] { "ConcurrencyStamp", "IsAdminUser", "PasswordHash" },
                values: new object[] { "30cf87c7-03c3-419a-8dbf-8f86aebbfdb3", false, "AQAAAAEAACcQAAAAEPKugYRsku98NX98pk6+DQ/oVtpZD24J9af+ApDHHyghVNLZ7dcX8jvvgtLYccMyiA==" });
        }
    }
}
