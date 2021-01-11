using Microsoft.EntityFrameworkCore.Migrations;

namespace IntlWallet.Data.Migrations
{
    public partial class AddedTransactionCurrency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TransactionCurrency",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionCurrency",
                table: "Transactions");
        }
    }
}
