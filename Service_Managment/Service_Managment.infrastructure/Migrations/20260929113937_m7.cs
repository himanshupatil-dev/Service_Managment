using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service_Managment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class m7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workers_Users_UserID",
                table: "Workers");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Workers",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Coinbalance",
                table: "Workers",
                newName: "CoinBalance");

            migrationBuilder.RenameColumn(
                name: "WorkerID",
                table: "Workers",
                newName: "WorkerId");

            migrationBuilder.RenameColumn(
                name: "IsCreated",
                table: "Workers",
                newName: "WorkerEmail");

            migrationBuilder.RenameIndex(
                name: "IX_Workers_UserID",
                table: "Workers",
                newName: "IX_Workers_UserId");

            migrationBuilder.AlterColumn<double>(
                name: "Rating",
                table: "Workers",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Workers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "Workers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Workers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Workers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "MobileNo",
                table: "Workers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Workers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Workers_Users_UserId",
                table: "Workers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workers_Users_UserId",
                table: "Workers");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Workers");

            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "Workers");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Workers");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Workers");

            migrationBuilder.DropColumn(
                name: "MobileNo",
                table: "Workers");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Workers");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Workers",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "CoinBalance",
                table: "Workers",
                newName: "Coinbalance");

            migrationBuilder.RenameColumn(
                name: "WorkerId",
                table: "Workers",
                newName: "WorkerID");

            migrationBuilder.RenameColumn(
                name: "WorkerEmail",
                table: "Workers",
                newName: "IsCreated");

            migrationBuilder.RenameIndex(
                name: "IX_Workers_UserId",
                table: "Workers",
                newName: "IX_Workers_UserID");

            migrationBuilder.AlterColumn<int>(
                name: "Rating",
                table: "Workers",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddForeignKey(
                name: "FK_Workers_Users_UserID",
                table: "Workers",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
