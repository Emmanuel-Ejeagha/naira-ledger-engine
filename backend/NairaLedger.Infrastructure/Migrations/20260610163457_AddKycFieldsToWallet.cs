using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NairaLedger.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddKycFieldsToWallet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KycFullName",
                table: "Wallets",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KycIdNumber",
                table: "Wallets",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KycIdType",
                table: "Wallets",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KycFullName",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "KycIdNumber",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "KycIdType",
                table: "Wallets");
        }
    }
}
