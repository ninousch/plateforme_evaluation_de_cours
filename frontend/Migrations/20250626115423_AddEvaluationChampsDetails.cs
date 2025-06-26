using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace frontend.Migrations
{
    /// <inheritdoc />
    public partial class AddEvaluationChampsDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Accessibilite",
                table: "Feedbacks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Clarte",
                table: "Feedbacks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommentaireFinal",
                table: "Feedbacks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Connaissances",
                table: "Feedbacks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Equite",
                table: "Feedbacks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeedbackNotes",
                table: "Feedbacks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Impact",
                table: "Feedbacks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Interet",
                table: "Feedbacks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Organisation",
                table: "Feedbacks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pertinence",
                table: "Feedbacks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Respect",
                table: "Feedbacks",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Accessibilite",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "Clarte",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "CommentaireFinal",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "Connaissances",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "Equite",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "FeedbackNotes",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "Impact",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "Interet",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "Organisation",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "Pertinence",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "Respect",
                table: "Feedbacks");
        }
    }
}
