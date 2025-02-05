using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DeskMarket.Data.Models;
using System.Reflection.Emit;

namespace DeskMarket.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ProductClient>()
                .HasKey(e => new { e.ProductId, e.ClientId });

            builder.Entity<ProductClient>()
               .HasOne(e => e.Product)
               .WithMany(e => e.ProductsClients)
               .HasForeignKey(e => e.ProductId)
               .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ProductClient>()
                .HasOne(e => e.Client)
                .WithMany()
                .HasForeignKey(e => e.ClientId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .Entity<Category>()
                .HasData(
                    new Category { Id = 1, Name = "Laptops" },
                    new Category { Id = 2, Name = "Workstations" },
                    new Category { Id = 3, Name = "Accessories" },
                    new Category { Id = 4, Name = "Desktops" },
                    new Category { Id = 5, Name = "Monitors" });
        }
        
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductClient> ProductsClients { get; set; }
    }
}
