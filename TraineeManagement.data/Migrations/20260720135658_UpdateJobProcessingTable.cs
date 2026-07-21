using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TraineeManagement.data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateJobProcessingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "JobStatus",
                table: "ProcessingJobs",
                newName: "Status");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 13, 56, 55, 585, DateTimeKind.Utc).AddTicks(5925), "$2a$11$9Bq3407G6xbQzv9ObjTsWuL.yX69to7c7i//ZLd/IK3MnhIpbvRg6", new DateTime(2026, 7, 20, 13, 56, 55, 585, DateTimeKind.Utc).AddTicks(6166) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "ProcessingJobs",
                newName: "JobStatus");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 9, 58, 42, 531, DateTimeKind.Utc).AddTicks(1088), "$2a$11$AYwYj3XjgPrYYuB9Rjz3fu5UgaiD6IbiWti69KmnK3SbYwMPZa3lu", new DateTime(2026, 7, 20, 9, 58, 42, 531, DateTimeKind.Utc).AddTicks(1307) });
        }
    }
}
