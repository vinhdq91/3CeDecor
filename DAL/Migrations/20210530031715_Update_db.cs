using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class Update_db : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DistrictCode",
                table: "Ward",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DistrictCode",
                table: "District",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2301D884-221A-4E7D-B509-0113DCC043E2",
                column: "ConcurrencyStamp",
                value: "b62a6d29-e776-46bf-a8e1-93d7b9264997");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "B22698B8-42A2-4115-9631-1C2D1E2AC5F8",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "3dfa86df-a9dd-489e-ba71-5f3e769176d5", "AQAAAAEAACcQAAAAEDoSBTOWWP6zd0NPr+vS5OYYEEbjSLlcNY+Ai9dz0U/tmnvf0Cm3+FAFVHJL+sM+Xw==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DistrictCode",
                table: "Ward",
                type: "nvarchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DistrictCode",
                table: "District",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2301D884-221A-4E7D-B509-0113DCC043E2",
                column: "ConcurrencyStamp",
                value: "647deb9c-20bf-4377-9a00-22b891dcc895");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "B22698B8-42A2-4115-9631-1C2D1E2AC5F8",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "5f16e79b-ddad-4757-a6eb-500920e0c96a", "AQAAAAEAACcQAAAAEBT0mJiKHeb4tDb2XpbGOxEtKYxdoUWv3/zjjYEVZhm2V+MBFD8/I5opNtSNq56ApA==" });
        }
    }
}
