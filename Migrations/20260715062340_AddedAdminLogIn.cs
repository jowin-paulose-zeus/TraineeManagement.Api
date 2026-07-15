using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TraineeManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedAdminLogIn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedDate", "Email", "PasswordHash", "Role", "UpdatedDate", "Username" },
                values: new object[] { 1, new DateTime(2026, 7, 15, 6, 23, 37, 666, DateTimeKind.Utc).AddTicks(3726), "admin@gmail.com", "$2a$11$UNsvx2/eJ8vd3.ZuBMaHOuo7ZOfuJYqlEl81QAlEhKGn//WTEtoCe", 0, new DateTime(2026, 7, 15, 6, 23, 37, 666, DateTimeKind.Utc).AddTicks(3963), "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
