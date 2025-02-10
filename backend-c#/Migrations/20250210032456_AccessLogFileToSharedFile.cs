using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_c_.Migrations
{
    /// <inheritdoc />
    public partial class AccessLogFileToSharedFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessLogs_Files_FileId",
                table: "AccessLogs");

            migrationBuilder.RenameColumn(
                name: "FileId",
                table: "AccessLogs",
                newName: "SharedFileId");

            migrationBuilder.RenameIndex(
                name: "IX_AccessLogs_FileId",
                table: "AccessLogs",
                newName: "IX_AccessLogs_SharedFileId");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Notifications",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessLogs_SharedFiles_SharedFileId",
                table: "AccessLogs",
                column: "SharedFileId",
                principalTable: "SharedFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessLogs_SharedFiles_SharedFileId",
                table: "AccessLogs");

            migrationBuilder.RenameColumn(
                name: "SharedFileId",
                table: "AccessLogs",
                newName: "FileId");

            migrationBuilder.RenameIndex(
                name: "IX_AccessLogs_SharedFileId",
                table: "AccessLogs",
                newName: "IX_AccessLogs_FileId");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Notifications",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AccessLogs_Files_FileId",
                table: "AccessLogs",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
