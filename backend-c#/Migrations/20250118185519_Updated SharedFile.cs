using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_c_.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedSharedFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Versions_Files_FileId",
                table: "Versions");

            migrationBuilder.DropForeignKey(
                name: "FK_Versions_Files_MediaFileId",
                table: "Versions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Versions",
                table: "Versions");

            migrationBuilder.RenameTable(
                name: "Versions",
                newName: "FileVersions");

            migrationBuilder.RenameIndex(
                name: "IX_Versions_MediaFileId",
                table: "FileVersions",
                newName: "IX_FileVersions_MediaFileId");

            migrationBuilder.RenameIndex(
                name: "IX_Versions_FileId",
                table: "FileVersions",
                newName: "IX_FileVersions_FileId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileVersions",
                table: "FileVersions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FileVersions_Files_FileId",
                table: "FileVersions",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FileVersions_Files_MediaFileId",
                table: "FileVersions",
                column: "MediaFileId",
                principalTable: "Files",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileVersions_Files_FileId",
                table: "FileVersions");

            migrationBuilder.DropForeignKey(
                name: "FK_FileVersions_Files_MediaFileId",
                table: "FileVersions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FileVersions",
                table: "FileVersions");

            migrationBuilder.RenameTable(
                name: "FileVersions",
                newName: "Versions");

            migrationBuilder.RenameIndex(
                name: "IX_FileVersions_MediaFileId",
                table: "Versions",
                newName: "IX_Versions_MediaFileId");

            migrationBuilder.RenameIndex(
                name: "IX_FileVersions_FileId",
                table: "Versions",
                newName: "IX_Versions_FileId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Versions",
                table: "Versions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Versions_Files_FileId",
                table: "Versions",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Versions_Files_MediaFileId",
                table: "Versions",
                column: "MediaFileId",
                principalTable: "Files",
                principalColumn: "Id");
        }
    }
}
