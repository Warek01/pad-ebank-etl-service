using System;
using Microsoft.EntityFrameworkCore.Migrations;
using PadEbankETLService.Data.DataWarehouse;
using PadEbankETLService.Data.Transaction;

#nullable disable

namespace PadEbankETLService.Data.DataWarehouse.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:public.currency", "mdl,usd,eur")
                .Annotation("Npgsql:Enum:public.transaction_type", "deposit,withdraw,transfer");

            migrationBuilder.CreateTable(
                name: "dim_cards",
                columns: table => new
                {
                    card_code = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    currency = table.Column<Currency>(type: "public.currency", nullable: false),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP WITHOUT TIME ZONE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dim_cards", x => x.card_code);
                });

            migrationBuilder.CreateTable(
                name: "dim_transactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    src_card_code = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    dst_card_code = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    type = table.Column<TransactionType>(type: "public.transaction_type", nullable: false),
                    currency = table.Column<Currency>(type: "public.currency", nullable: false),
                    amount = table.Column<double>(type: "double precision", nullable: false),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP WITHOUT TIME ZONE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dim_transactions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "dim_users",
                columns: table => new
                {
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    full_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP WITHOUT TIME ZONE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dim_users", x => x.email);
                });

            migrationBuilder.CreateTable(
                name: "fact_card_transactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    currency = table.Column<Currency>(type: "public.currency", nullable: false),
                    amount = table.Column<double>(type: "double precision", nullable: false),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP WITHOUT TIME ZONE", nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    dst_card_code = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fact_card_transactions", x => x.id);
                    table.ForeignKey(
                        name: "FK_fact_card_transactions_dim_cards_dst_card_code",
                        column: x => x.dst_card_code,
                        principalTable: "dim_cards",
                        principalColumn: "card_code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_fact_card_transactions_dim_transactions_id",
                        column: x => x.id,
                        principalTable: "dim_transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_fact_card_transactions_dim_users_email",
                        column: x => x.email,
                        principalTable: "dim_users",
                        principalColumn: "email",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_fact_card_transactions_dst_card_code",
                table: "fact_card_transactions",
                column: "dst_card_code");

            migrationBuilder.CreateIndex(
                name: "IX_fact_card_transactions_email",
                table: "fact_card_transactions",
                column: "email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fact_card_transactions");

            migrationBuilder.DropTable(
                name: "dim_cards");

            migrationBuilder.DropTable(
                name: "dim_transactions");

            migrationBuilder.DropTable(
                name: "dim_users");
        }
    }
}
