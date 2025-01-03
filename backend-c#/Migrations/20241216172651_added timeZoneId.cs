using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_c_.Migrations
{
    /// <inheritdoc />
    public partial class addedtimeZoneId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessLogs_Files_FileId1",
                table: "AccessLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_SharedFiles_Files_FileId1",
                table: "SharedFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Versions_Files_FileId1",
                table: "Versions");

            migrationBuilder.RenameColumn(
                name: "FileId1",
                table: "Versions",
                newName: "MediaFileId");

            migrationBuilder.RenameIndex(
                name: "IX_Versions_FileId1",
                table: "Versions",
                newName: "IX_Versions_MediaFileId");

            migrationBuilder.RenameColumn(
                name: "FileId1",
                table: "SharedFiles",
                newName: "MediaFileId");

            migrationBuilder.RenameIndex(
                name: "IX_SharedFiles_FileId1",
                table: "SharedFiles",
                newName: "IX_SharedFiles_MediaFileId");

            migrationBuilder.RenameColumn(
                name: "FileId1",
                table: "AccessLogs",
                newName: "MediaFileId");

            migrationBuilder.RenameIndex(
                name: "IX_AccessLogs_FileId1",
                table: "AccessLogs",
                newName: "IX_AccessLogs_MediaFileId");

            migrationBuilder.AddColumn<string>(
                name: "TimeZoneId",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessLogs_Files_MediaFileId",
                table: "AccessLogs",
                column: "MediaFileId",
                principalTable: "Files",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SharedFiles_Files_MediaFileId",
                table: "SharedFiles",
                column: "MediaFileId",
                principalTable: "Files",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Versions_Files_MediaFileId",
                table: "Versions",
                column: "MediaFileId",
                principalTable: "Files",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessLogs_Files_MediaFileId",
                table: "AccessLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_SharedFiles_Files_MediaFileId",
                table: "SharedFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Versions_Files_MediaFileId",
                table: "Versions");

            migrationBuilder.DropColumn(
                name: "TimeZoneId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "MediaFileId",
                table: "Versions",
                newName: "FileId1");

            migrationBuilder.RenameIndex(
                name: "IX_Versions_MediaFileId",
                table: "Versions",
                newName: "IX_Versions_FileId1");

            migrationBuilder.RenameColumn(
                name: "MediaFileId",
                table: "SharedFiles",
                newName: "FileId1");

            migrationBuilder.RenameIndex(
                name: "IX_SharedFiles_MediaFileId",
                table: "SharedFiles",
                newName: "IX_SharedFiles_FileId1");

            migrationBuilder.RenameColumn(
                name: "MediaFileId",
                table: "AccessLogs",
                newName: "FileId1");

            migrationBuilder.RenameIndex(
                name: "IX_AccessLogs_MediaFileId",
                table: "AccessLogs",
                newName: "IX_AccessLogs_FileId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessLogs_Files_FileId1",
                table: "AccessLogs",
                column: "FileId1",
                principalTable: "Files",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SharedFiles_Files_FileId1",
                table: "SharedFiles",
                column: "FileId1",
                principalTable: "Files",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Versions_Files_FileId1",
                table: "Versions",
                column: "FileId1",
                principalTable: "Files",
                principalColumn: "Id");
        }
    }
}
