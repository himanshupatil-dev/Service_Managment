using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service_Managment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class M12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Otp",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "OtpExpiresAt",
                table: "Appointments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "OtpHash",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Otp",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "OtpExpiresAt",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "OtpHash",
                table: "Appointments");
        }
    }
}
