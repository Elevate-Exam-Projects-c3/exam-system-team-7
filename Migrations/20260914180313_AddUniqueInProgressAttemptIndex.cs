using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace exam_system.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueInProgressAttemptIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_QuizAttempts_StudentId_QuizId",
                table: "QuizAttempts");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Quizzes",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PassScore",
                table: "Quizzes",
                type: "int",
                nullable: true,
                defaultValue: 60,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_StudentId_QuizId",
                table: "QuizAttempts",
                columns: new[] { "StudentId", "QuizId" },
                unique: true,
                filter: "[Status] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_QuizAttempts_StudentId_QuizId",
                table: "QuizAttempts");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Quizzes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "PassScore",
                table: "Quizzes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValue: 60);

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_StudentId_QuizId",
                table: "QuizAttempts",
                columns: new[] { "StudentId", "QuizId" });
        }
    }
}
