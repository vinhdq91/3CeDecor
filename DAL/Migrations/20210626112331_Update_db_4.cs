using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class Update_db_4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppProducts_AppProductCategories_ProductCategoryId",
                table: "AppProducts");

            migrationBuilder.DropIndex(
                name: "IX_AppProducts_ProductCategoryId",
                table: "AppProducts");

            migrationBuilder.DropColumn(
                name: "ProductCategoryId",
                table: "AppProducts");

            migrationBuilder.CreateTable(
                name: "ProductProductCategories",
                columns: table => new
                {
                    ProductId = table.Column<int>(nullable: false),
                    ProductCategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductProductCategories", x => new { x.ProductId, x.ProductCategoryId });
                    table.ForeignKey(
                        name: "FK_ProductProductCategories_AppProductCategories_ProductCategoryId",
                        column: x => x.ProductCategoryId,
                        principalTable: "AppProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductProductCategories_AppProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "AppProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2301D884-221A-4E7D-B509-0113DCC043E2",
                column: "ConcurrencyStamp",
                value: "3d77133a-ace0-498d-aeac-27e1a902f361");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "B22698B8-42A2-4115-9631-1C2D1E2AC5F8",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "132aedea-a956-46c4-ae64-18dd9f41ddd0", "AQAAAAEAACcQAAAAEHBBfzGmaFoBjHyigj/d1jIpuoWOQn6qGb8LkzDHaszDsijPzfohlqoeIkmnPG3wxw==" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductProductCategories_ProductCategoryId",
                table: "ProductProductCategories",
                column: "ProductCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductProductCategories");

            migrationBuilder.AddColumn<int>(
                name: "ProductCategoryId",
                table: "AppProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.CreateIndex(
                name: "IX_AppProducts_ProductCategoryId",
                table: "AppProducts",
                column: "ProductCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppProducts_AppProductCategories_ProductCategoryId",
                table: "AppProducts",
                column: "ProductCategoryId",
                principalTable: "AppProductCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
