using Microsoft.EntityFrameworkCore;

namespace GeekShoping.ProductAPI.Model.Context
{
    public class MySQLContext : DbContext
    {
        public MySQLContext() { }
        public MySQLContext(DbContextOptions<MySQLContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 4,
                    Name = "Caderno",
                    Price = 10.99M,
                    Description = "Caderno de anotações",
                    CategoryName = "Papelaria",
                    ImageURL = "https://example.com/caderno.jpg"
                },
                new Product
                {
                    Id = 5,
                    Name = "Caneta",
                    Price = 2.99M,
                    Description = "Caneta esferográfica",
                    CategoryName = "Papelaria",
                    ImageURL = "https://example.com/caneta.jpg"
                },
                new Product
                {
                    Id = 6,
                    Name = "Mochila",
                    Price = 49.99M,
                    Description = "Mochila para laptop",
                    CategoryName = "Acessórios",
                    ImageURL = "https://example.com/mochila.jpg"
                },
                new Product
                {
                    Id = 7,
                    Name = "Fone de Ouvido",
                    Price = 29.99M,
                    Description = "Fone de ouvido sem fio",
                    CategoryName = "Eletrônicos",
                    ImageURL = "https://example.com/fone.jpg"
                },
                new Product
                {
                    Id = 8,
                    Name = "Garrafa Térmica",
                    Price = 19.99M,
                    Description = "Garrafa térmica para bebidas quentes ou frias",
                    CategoryName = "Acessórios",
                    ImageURL = "https://example.com/garrafa.jpg"
                },
                new Product
                {
                    Id = 9,
                    Name = "Relógio de Pulso",
                    Price = 99.99M,
                    Description = "Relógio de pulso com design moderno",
                    CategoryName = "Acessórios",
                    ImageURL = "https://example.com/relogio.jpg"
                },
                new Product
                {
                    Id = 10,
                    Name = "Camiseta",
                    Price = 14.99M,
                    Description = "Camiseta de algodão confortável",
                    CategoryName = "Roupas",
                    ImageURL = "https://example.com/camiseta.jpg"
                }
            );
        }
    }
}
