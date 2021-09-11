using Microsoft.EntityFrameworkCore.Migrations;

namespace ImageRepositoryW22.Migrations
{
    public partial class AddOcrTextToDatabaseImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageText",
                table: "Images",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageText",
                table: "Images");
        }
    }
}
