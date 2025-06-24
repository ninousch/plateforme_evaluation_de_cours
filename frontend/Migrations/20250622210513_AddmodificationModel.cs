using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace frontend.Migrations
{
    /// <inheritdoc />
    public partial class AddmodificationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_SessionsFeedback_SessionFeedbackId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_SessionFeedbackId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "SessionFeedbackId",
                table: "Feedbacks");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_SessionId",
                table: "Feedbacks",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Sessions_SessionId",
                table: "Feedbacks",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Sessions_SessionId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_SessionId",
                table: "Feedbacks");

            migrationBuilder.AddColumn<int>(
                name: "SessionFeedbackId",
                table: "Feedbacks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_SessionFeedbackId",
                table: "Feedbacks",
                column: "SessionFeedbackId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_SessionsFeedback_SessionFeedbackId",
                table: "Feedbacks",
                column: "SessionFeedbackId",
                principalTable: "SessionsFeedback",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
