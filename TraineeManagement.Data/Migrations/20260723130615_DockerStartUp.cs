using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TraineeManagement.data.Migrations
{
    /// <inheritdoc />
    public partial class DockerStartUp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 7, 23, 13, 6, 13, 739, DateTimeKind.Utc).AddTicks(2908), "$2a$11$0rVWE3m3x4pypF3Tq5fDpu9Jlbmlyii1P9Rm3Uts6LOITzmFoCK32", new DateTime(2026, 7, 23, 13, 6, 13, 739, DateTimeKind.Utc).AddTicks(3184) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 7, 20, 13, 56, 55, 585, DateTimeKind.Utc).AddTicks(5925), "$2a$11$9Bq3407G6xbQzv9ObjTsWuL.yX69to7c7i//ZLd/IK3MnhIpbvRg6", new DateTime(2026, 7, 20, 13, 56, 55, 585, DateTimeKind.Utc).AddTicks(6166) });
        }
    }
}
