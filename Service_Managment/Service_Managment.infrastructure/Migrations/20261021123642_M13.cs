using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service_Managment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class M13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerifiedAt",
                table: "OtpVerifications");

            migrationBuilder.DropColumn(
                name: "VerifiedBy",
                table: "OtpVerifications");

            migrationBuilder.DropColumn(
                name: "Otp",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "OtpExpiresAt",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "OtpHash",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "IsVerified",
                table: "OtpVerifications",
                newName: "IsUsed");

            migrationBuilder.RenameColumn(
                name: "GeneratedAt",
                table: "OtpVerifications",
                newName: "ExpiresAt");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "OtpVerifications",
                newName: "OtpHash");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OtpVerifications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "FailedAttempts",
                table: "OtpVerifications",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OtpVerifications");

            migrationBuilder.DropColumn(
                name: "FailedAttempts",
                table: "OtpVerifications");

            migrationBuilder.RenameColumn(
                name: "OtpHash",
                table: "OtpVerifications",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "IsUsed",
                table: "OtpVerifications",
                newName: "IsVerified");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "OtpVerifications",
                newName: "GeneratedAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedAt",
                table: "OtpVerifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VerifiedBy",
                table: "OtpVerifications",
                type: "int",
                nullable: true);

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
    }
}
