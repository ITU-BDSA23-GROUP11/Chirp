using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImproveLikesDirectRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Likes",
                table: "Likes");

            migrationBuilder.RenameColumn(
                name: "LikedByAuthorId",
                table: "Likes",
                newName: "LikeId");

            migrationBuilder.AlterColumn<Guid>(
                name: "CheepId",
                table: "Likes",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "AuthorId",
                table: "Likes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Likes",
                table: "Likes",
                column: "LikeId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_AuthorId",
                table: "Likes",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_CheepId",
                table: "Likes",
                column: "CheepId");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Authors_AuthorId",
                table: "Likes",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "AuthorId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Cheeps_CheepId",
                table: "Likes",
                column: "CheepId",
                principalTable: "Cheeps",
                principalColumn: "CheepId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Authors_AuthorId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Cheeps_CheepId",
                table: "Likes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Likes",
                table: "Likes");

            migrationBuilder.DropIndex(
                name: "IX_Likes_AuthorId",
                table: "Likes");

            migrationBuilder.DropIndex(
                name: "IX_Likes_CheepId",
                table: "Likes");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Likes");

            migrationBuilder.RenameColumn(
                name: "LikeId",
                table: "Likes",
                newName: "LikedByAuthorId");

            migrationBuilder.AlterColumn<Guid>(
                name: "CheepId",
                table: "Likes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Likes",
                table: "Likes",
                columns: new[] { "LikedByAuthorId", "CheepId" });
        }
    }
}
