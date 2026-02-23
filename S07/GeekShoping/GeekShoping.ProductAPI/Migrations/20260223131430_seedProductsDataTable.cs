using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GeekShoping.ProductAPI.Migrations
{
    /// <inheritdoc />
    public partial class seedProductsDataTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TB_Product",
                columns: new[] { "id", "CategoryName", "Description", "ImageURL", "name", "Price" },
                values: new object[,]
                {
                    { 4L, "Papelaria", "Caderno de anotações", "https://example.com/caderno.jpg", "Caderno", 10.99m },
                    { 5L, "Papelaria", "Caneta esferográfica", "https://example.com/caneta.jpg", "Caneta", 2.99m },
                    { 6L, "Acessórios", "Mochila para laptop", "https://example.com/mochila.jpg", "Mochila", 49.99m },
                    { 7L, "Eletrônicos", "Fone de ouvido sem fio", "https://example.com/fone.jpg", "Fone de Ouvido", 29.99m },
                    { 8L, "Acessórios", "Garrafa térmica para bebidas quentes ou frias", "https://example.com/garrafa.jpg", "Garrafa Térmica", 19.99m },
                    { 9L, "Acessórios", "Relógio de pulso com design moderno", "https://example.com/relogio.jpg", "Relógio de Pulso", 99.99m },
                    { 10L, "Roupas", "Camiseta de algodão confortável", "https://example.com/camiseta.jpg", "Camiseta", 14.99m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TB_Product",
                keyColumn: "id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "TB_Product",
                keyColumn: "id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "TB_Product",
                keyColumn: "id",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "TB_Product",
                keyColumn: "id",
                keyValue: 7L);

            migrationBuilder.DeleteData(
                table: "TB_Product",
                keyColumn: "id",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "TB_Product",
                keyColumn: "id",
                keyValue: 9L);

            migrationBuilder.DeleteData(
                table: "TB_Product",
                keyColumn: "id",
                keyValue: 10L);
        }
    }
}
