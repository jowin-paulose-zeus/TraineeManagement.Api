using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace TraineeManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedSubmissionFileTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubmissionFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    SubmissionId = table.Column<int>(type: "int", nullable: false),
                    OriginalFileName = table.Column<string>(type: "longtext", nullable: false),
                    StoredFileName = table.Column<string>(type: "longtext", nullable: false),
                    ContentType = table.Column<string>(type: "longtext", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    Checksum = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmissionFiles_Submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 7, 15, 11, 15, 0, 864, DateTimeKind.Utc).AddTicks(8552), "$2a$11$5C1qYOXa279eyNYY4DzMg.rQp6dC8Jipz/kH5RXYnH.bJsPKGYHj6", new DateTime(2026, 7, 15, 11, 15, 0, 864, DateTimeKind.Utc).AddTicks(8768) });

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionFiles_SubmissionId",
                table: "SubmissionFiles",
                column: "SubmissionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubmissionFiles");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 7, 15, 6, 23, 37, 666, DateTimeKind.Utc).AddTicks(3726), "$2a$11$UNsvx2/eJ8vd3.ZuBMaHOuo7ZOfuJYqlEl81QAlEhKGn//WTEtoCe", new DateTime(2026, 7, 15, 6, 23, 37, 666, DateTimeKind.Utc).AddTicks(3963) });
        }
    }
}
