using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TraineeManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSubmissionConstructor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MentorId",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_MentorId",
                table: "Reviews",
                column: "MentorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Mentors_MentorId",
                table: "Reviews",
                column: "MentorId",
                principalTable: "Mentors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Mentors_MentorId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_MentorId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "MentorId",
                table: "Reviews");
        }
    }
}
