using E_commerce.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using E_commerce.Models;

namespace E_commerce.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Payment> Payments => Set<Payment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cart>().HasMany(c => c.Items).WithOne(i => i.Cart).HasForeignKey(i => i.CartId);
            modelBuilder.Entity<Order>().HasMany(o => o.Items).WithOne(i => i.Order).HasForeignKey(i => i.OrderId);
            base.OnModelCreating(modelBuilder);
        }
    }

    public static class DemoData
    {
        public static void Seed(AppDbContext db)
        {
            if (!db.Products.Any())
            {
                db.Products.AddRange(new[]
                {
                    new Product { Name = "T-Shirt", Description = "Cotton t-shirt", Price = 1999, Currency = "usd" },
                    new Product { Name = "Coffee Mug", Description = "Ceramic mug", Price = 999, Currency = "usd" },
                    new Product { Name = "Notebook", Description = "Hardcover notebook", Price = 499, Currency = "usd" }
                });
                db.SaveChanges();
            }
        }
    }
}
