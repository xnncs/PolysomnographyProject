using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolysomnographyProject.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UniqueLogin = table.Column<string>(type: "text", nullable: false),
                    TelegramUserData_TelegramId = table.Column<long>(type: "bigint", nullable: false),
                    TelegramUserData_TelegramUsername = table.Column<string>(type: "text", nullable: false),
                    PersonalSleepData_SleepTimePreferences_StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PersonalSleepData_SleepTimePreferences_EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SleepResult",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Data_SDNN = table.Column<double>(type: "double precision", nullable: false),
                    Data_LF = table.Column<double>(type: "double precision", nullable: false),
                    Data_HF = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SleepResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SleepResult_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SleepResult_UserId",
                table: "SleepResult",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SleepResult");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
