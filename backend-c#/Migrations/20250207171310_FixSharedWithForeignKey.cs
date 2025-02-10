using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_c_.Migrations
{
    /// <inheritdoc />
    public partial class FixSharedWithForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SharedFiles_Users_OwnerId",
                table: "SharedFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_SharedFiles_Users_SharedWithId",
                table: "SharedFiles");

            migrationBuilder.AlterColumn<int>(
                name: "SharedWithId",
                table: "SharedFiles",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "SharedFiles",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "FileId",
                table: "SharedFiles",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Notifications",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Notifications",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId1",
                table: "Notifications",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_UserId1",
                table: "Notifications",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SharedFiles_Users_OwnerId",
                table: "SharedFiles",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SharedFiles_Users_SharedWithId",
                table: "SharedFiles",
                column: "SharedWithId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Users_UserId1",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_SharedFiles_Users_OwnerId",
                table: "SharedFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_SharedFiles_Users_SharedWithId",
                table: "SharedFiles");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_UserId1",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Notifications");

            migrationBuilder.AlterColumn<int>(
                name: "SharedWithId",
                table: "SharedFiles",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "SharedFiles",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FileId",
                table: "SharedFiles",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Notifications",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SharedFiles_Users_OwnerId",
                table: "SharedFiles",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SharedFiles_Users_SharedWithId",
                table: "SharedFiles",
                column: "SharedWithId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
