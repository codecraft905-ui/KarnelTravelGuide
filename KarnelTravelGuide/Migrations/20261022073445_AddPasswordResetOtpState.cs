using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KarnelTravelGuide.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordResetOtpState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PasswordResetAttempts",
                table: "UserAccounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "PasswordResetVerified",
                table: "UserAccounts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordResetAttempts",
                table: "UserAccounts");

            migrationBuilder.DropColumn(
                name: "PasswordResetVerified",
                table: "UserAccounts");
        }
    }
}
